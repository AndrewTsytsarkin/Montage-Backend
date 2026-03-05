using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MontageAPI.Data;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminObjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminObjectsController(AppDbContext context) => _context = context;

        /// <summary>
        /// Получить все объекты с информацией о назначениях (только Admin)
        /// </summary>
        [HttpGet("objects")]
        public async Task<IActionResult> GetAllObjects()
        {
            var objects = await _context.ProjectObjects
                .Include(o => o.Assignments)
                    .ThenInclude(a => a.User)
                .Select(o => new ProjectObjectDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Address = o.Address,
                    Status = o.Status,
                    Description = o.Description,
                    CreatedAt = o.CreatedAt,
                    AssignedUsers = o.Assignments
                        .Select(a => new AssignedUserDto
                        {
                            UserId = a.User.Id,
                            Login = a.User.Login,
                            FullName = a.User.FullName
                        })
                        .ToList()
                })
                .OrderBy(o => o.Name)
                .ToListAsync();

            return Ok(objects);
        }

        /// <summary>
        /// Получить объект с полными данными для редактирования (только Admin)
        /// </summary>
        [HttpGet("objects/{id}")]
        public async Task<IActionResult> GetObjectForEdit(int id)
        {
            var obj = await _context.ProjectObjects
                .Include(o => o.Assignments)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (obj == null)
                return NotFound();

            // Все пользователи кроме админов (монтажников можно назначать)
            var allWorkers = await _context.Users
                .Where(u => u.Role == "Worker")
                .Select(u => new AvailableUserDto
                {
                    Id = u.Id,
                    Login = u.Login,
                    FullName = u.FullName,
                    IsAssigned = false  // Заполним ниже
                })
                .ToListAsync();

            // IDs уже назначенных пользователей
            var assignedUserIds = obj.Assignments.Select(a => a.UserId).ToHashSet();

            // Отмечаем кто уже назначен
            foreach (var user in allWorkers)
            {
                user.IsAssigned = assignedUserIds.Contains(user.Id);
            }

            var result = new ProjectObjectDto
            {
                Id = obj.Id,
                Name = obj.Name,
                Address = obj.Address,
                Status = obj.Status,
                Description = obj.Description,
                CreatedAt = obj.CreatedAt,
                AssignedUsers = obj.Assignments
                    .Select(a => new AssignedUserDto
                    {
                        UserId = a.User.Id,
                        Login = a.User.Login,
                        FullName = a.User.FullName
                    })
                    .ToList(),
                AvailableUsers = allWorkers
            };

            return Ok(result);
        }

        /// <summary>
        /// Создать новый объект (только Admin)
        /// </summary>
        [HttpPost("objects")]
        public async Task<IActionResult> CreateObject([FromBody] CreateUpdateObjectDto dto)
        {
            var obj = new ProjectObject
            {
                Name = dto.Name,
                Address = dto.Address,
                Status = dto.Status,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProjectObjects.Add(obj);
            await _context.SaveChangesAsync();

            // Назначаем пользователей если указаны
            if (dto.AssignedUserIds?.Any() == true)
            {
                foreach (var userId in dto.AssignedUserIds)
                {
                    // Проверяем что пользователь существует и не админ
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == userId && u.Role == "Worker");

                    if (user != null)
                    {
                        _context.UserObjectAssignments.Add(new UserObjectAssignment
                        {
                            UserId = userId,
                            ObjectId = obj.Id
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetAllObjects), new { id = obj.Id }, new
            {
                id = obj.Id,
                message = "Объект создан"
            });
        }

        /// <summary>
        /// Обновить объект и назначения (только Admin)
        /// </summary>
        [HttpPut("objects/{id}")]
        public async Task<IActionResult> UpdateObject(int id, [FromBody] CreateUpdateObjectDto dto)
        {
            var obj = await _context.ProjectObjects
                .Include(o => o.Assignments)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (obj == null)
                return NotFound();

            // Обновляем основные поля
            obj.Name = dto.Name;
            obj.Address = dto.Address;
            obj.Status = dto.Status;
            obj.Description = dto.Description;
            obj.UpdatedAt = DateTime.UtcNow;

            // Обновляем назначения пользователей
            if (dto.AssignedUserIds != null)
            {
                // Удаляем старые назначения
                var toRemove = obj.Assignments
                    .Where(a => !dto.AssignedUserIds.Contains(a.UserId))
                    .ToList();

                foreach (var assignment in toRemove)
                {
                    _context.UserObjectAssignments.Remove(assignment);
                }

                // Добавляем новые назначения
                var existingUserIds = obj.Assignments.Select(a => a.UserId).ToHashSet();

                foreach (var userId in dto.AssignedUserIds)
                {
                    if (!existingUserIds.Contains(userId))
                    {
                        // Проверяем что пользователь существует
                        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                        if (userExists)
                        {
                            _context.UserObjectAssignments.Add(new UserObjectAssignment
                            {
                                UserId = userId,
                                ObjectId = id
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Объект обновлён" });
        }

        /// <summary>
        /// Удалить объект (только Admin)
        /// </summary>
        [HttpDelete("objects/{id}")]
        public async Task<IActionResult> DeleteObject(int id)
        {
            var obj = await _context.ProjectObjects
                .Include(o => o.Assignments)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (obj == null)
                return NotFound();

            // Проверка: нельзя удалить объект с выполненными работами
            var hasWorks = await _context.WorkReports.AnyAsync(w => w.ObjectId == id);
            if (hasWorks)
            {
                return BadRequest(new { message = "Нельзя удалить объект с выполненными работами" });
            }

            // Удаляем назначения
            _context.UserObjectAssignments.RemoveRange(obj.Assignments);

            // Удаляем объект
            _context.ProjectObjects.Remove(obj);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Объект удалён" });
        }

        /// <summary>
        /// Получить список всех монтажников для назначения (только Admin)
        /// </summary>
        [HttpGet("workers")]
        public async Task<IActionResult> GetAllWorkers()
        {
            var workers = await _context.Users
                .Where(u => u.Role == "Worker")
                .Select(u => new
                {
                    id = u.Id,
                    login = u.Login,
                    fullName = u.FullName
                })
                .OrderBy(u => u.login)
                .ToListAsync();

            return Ok(workers);
        }
    }
}
