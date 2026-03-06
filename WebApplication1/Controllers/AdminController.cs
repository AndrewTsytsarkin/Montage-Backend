using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MontageAPI.Data;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context) => _context = context;

        /// <summary>
        /// Получить всех пользователей (только Admin)
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Login = u.Login,
                    FullName = u.FullName,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .OrderBy(u => u.Login)
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Создать нового пользователя (только Admin)
        /// </summary>
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            // Проверка: логин уже существует
            if (await _context.Users.AnyAsync(u => u.Login == dto.Login))
                return BadRequest(new { message = "Пользователь с таким логином уже существует" });

            // Проверка: роль должна быть Admin или Worker
            if (dto.Role != "Admin" && dto.Role != "Worker")
                return BadRequest(new { message = "Недопустимая роль" });

            var user = new User
            {
                Login = dto.Login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, new
            {
                id = user.Id,
                login = user.Login,
                role = user.Role,      // ✅ РОЛЬ В ОТВЕТЕ
                message = "Пользователь создан"
            });
        }

        /// <summary>
        /// Обновить пользователя (только Admin)
        /// </summary>
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            // Нельзя редактировать самого себя через этот метод
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (user.Id == currentUserId)
                return BadRequest(new { message = "Для редактирования своего профиля используйте другой endpoint" });

            // ✅ Обновление логина (если предоставлен)
            if (!string.IsNullOrEmpty(dto.Login))
            {
                // Проверка: логин уже существует у другого пользователя
                var loginExists = await _context.Users.AnyAsync(u => u.Login == dto.Login && u.Id != id);
                if (loginExists)
                    return BadRequest(new { message = "Пользователь с таким логином уже существует" });

                // Проверка: формат логина (минимум 3 символа, только буквы/цифры/подчёркивание)
                if (dto.Login.Length < 3)
                    return BadRequest(new { message = "Логин должен содержать минимум 3 символа" });

                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Login, @"^[a-zA-Z0-9_]+$"))
                    return BadRequest(new { message = "Логин может содержать только буквы, цифры и подчёркивание" });

                user.Login = dto.Login;
            }

            // ✅ Обновление пароля (если предоставлен)
            if (!string.IsNullOrEmpty(dto.Password))
            {
                // Проверка: минимальная длина пароля
                if (dto.Password.Length < 6)
                    return BadRequest(new { message = "Пароль должен содержать минимум 6 символов" });

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            // Обновление ФИО
            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName;

            // Обновление роли
            if (!string.IsNullOrEmpty(dto.Role) && (dto.Role == "Admin" || dto.Role == "Worker"))
            {
                // Нельзя удалить последнего админа
                if (user.Role == "Admin" && dto.Role == "Worker")
                {
                    var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
                    if (adminCount <= 1)
                        return BadRequest(new { message = "Нельзя понизить последнего администратора" });
                }

                user.Role = dto.Role;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользователь обновлён" });
        }

        /// <summary>
        /// Удалить пользователя (только Admin)
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            // Нельзя удалить самого себя
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (user.Id == currentUserId)
                return BadRequest(new { message = "Нельзя удалить самого себя" });

            // Нельзя удалить последнего админа
            if (user.Role == "Admin")
            {
                var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
                if (adminCount <= 1)
                    return BadRequest(new { message = "Нельзя удалить последнего администратора" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользователь удалён" });
        }

        /// <summary>
        /// Получить статистику (только Admin)
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
            var workerCount = await _context.Users.CountAsync(u => u.Role == "Worker");
            var totalWorks = await _context.WorkReports.CountAsync();
            var totalObjects = await _context.ProjectObjects.CountAsync();

            return Ok(new
            {
                totalUsers,
                adminCount,
                workerCount,
                totalWorks,
                totalObjects
            });
        }
    }
}
