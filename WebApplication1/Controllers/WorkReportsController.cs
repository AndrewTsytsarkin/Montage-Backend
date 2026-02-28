using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MontageAPI.Data;

public class UserDto
    {
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Role { get; set; } = string.Empty;
}
public class WorkReportDto
{
    public int Id { get; set; }

    public string? UserFullName { get; set; }  // ✅ НОВОЕ
    public int UserId { get; set; }
    public string UserLogin { get; set; } = string.Empty;
    public int ObjectId { get; set; }
    public string ObjectName { get; set; } = string.Empty;
    public int WorkTypeId { get; set; }
    public string WorkTypeName { get; set; } = string.Empty;
    public string WorkTypeType { get; set; } = string.Empty;
    public string WorkTypeSubtype { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWorkReportDto
{
    public int ObjectId { get; set; }
    public int WorkTypeId { get; set; }
    public DateTime WorkDate { get; set; }
    public decimal Quantity { get; set; }
    public string? Comment { get; set; }
}

public class UpdateWorkReportDto
{
    public decimal? Quantity { get; set; }
    public string? Comment { get; set; }
}

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkReportsController(AppDbContext context) => _context = context;

        /// <summary>
        /// Получить отчеты (Админ - все, Монтажник - только свои)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetReports(
            [FromQuery] int? objectId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int? userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IQueryable<WorkReport> query = _context.WorkReports
                .Include(r => r.User)
                .Include(r => r.Object)
                .Include(r => r.WorkType);

            // Разграничение доступа
            if (currentUserRole != "Admin")
            {
                // Монтажник видит только свои отчеты
                query = query.Where(r => r.UserId == currentUserId);

                // И только по своим объектам
                query = query.Where(r => _context.UserObjectAssignments
                    .Any(a => a.ObjectId == r.ObjectId && a.UserId == currentUserId));
            }
            else if (userId.HasValue)
            {
                // Админ может фильтровать по конкретному монтажнику
                query = query.Where(r => r.UserId == userId.Value);
            }

            // Фильтры
            if (objectId.HasValue)
                query = query.Where(r => r.ObjectId == objectId.Value);

            if (fromDate.HasValue)
                query = query.Where(r => r.WorkDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(r => r.WorkDate <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            var reports = await query
                .OrderByDescending(r => r.WorkDate)
                .ThenByDescending(r => r.CreatedAt)
                .Select(r => new WorkReportDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserLogin = r.User.Login,
                    UserFullName = r.User.FullName,  // ✅ НОВОЕ
                    ObjectId = r.ObjectId,
                    ObjectName = r.Object.Name,
                    WorkTypeId = r.WorkTypeId,
                    WorkTypeName = r.WorkType.Name,
                    WorkTypeType = r.WorkType.Type,
                    WorkTypeSubtype = r.WorkType.Subtype,
                    WorkDate = r.WorkDate,
                    Quantity = r.Quantity,
                    PricePerUnit = r.PricePerUnit,
                    TotalPrice = r.TotalPrice,
                    Unit = r.Unit,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();

            return Ok(reports);
        }


        /// <summary>
        /// Удалить отчет
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var report = await _context.WorkReports.FindAsync(id);
            if (report == null)
                return NotFound();

            // Проверка прав: только автор или админ может удалять
            if (report.UserId != currentUserId && currentUserRole != "Admin")
                return Forbid();

            _context.WorkReports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Получить статистику по работам (для админа)
        /// </summary>
        [HttpGet("stats/summary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSummaryStats(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.WorkReports
                .Include(r => r.WorkType)
                .Include(r => r.User);

            if (fromDate.HasValue)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<WorkReport, User>)query.Where(r => r.WorkDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<WorkReport, User>)query.Where(r => r.WorkDate <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            var stats = await query
                .GroupBy(r => new { r.WorkTypeId, r.WorkType.Name, r.WorkType.Unit })
                .Select(g => new
                {
                    WorkTypeId = g.Key.WorkTypeId,
                    WorkName = g.Key.Name,
                    Unit = g.Key.Unit,
                    TotalQuantity = g.Sum(r => r.Quantity),
                    ReportCount = g.Count()
                })
                .ToListAsync();

            return Ok(stats);
        }




        /// <summary>
        /// Создать отчет о выполненной работе
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] CreateWorkReportDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Проверка доступа к объекту
            var hasAccess = await _context.UserObjectAssignments
                .AnyAsync(a => a.ObjectId == dto.ObjectId && a.UserId == currentUserId);

            if (!hasAccess && currentUserRole != "Admin")
                return Forbid("Нет доступа к этому объекту");

            // Получаем вид работы с ценой
            var workType = await _context.WorkTypes.FindAsync(dto.WorkTypeId);
            if (workType == null)
                return BadRequest("Вид работы не найден");

            // === АВТОМАТИЧЕСКИЙ РАСЧЕТ СТОИМОСТИ ===
            var totalPrice = workType.PricePerUnit * dto.Quantity;

            var report = new WorkReport
            {
                UserId = currentUserId,
                ObjectId = dto.ObjectId,
                WorkTypeId = dto.WorkTypeId,
                WorkDate = dto.WorkDate.Date,
                Quantity = dto.Quantity,
                Unit = workType.Unit,
                PricePerUnit = workType.PricePerUnit, // Копируем цену на момент создания
                TotalPrice = totalPrice,              // Расчет итоговой суммы
                Comment = dto.Comment
            };

            _context.WorkReports.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReports), new { id = report.Id }, report.Id);
        }

        /// <summary>
        /// Обновить отчет (пересчитываем стоимость при изменении количества)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] UpdateWorkReportDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var report = await _context.WorkReports
                .Include(r => r.WorkType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
                return NotFound();

            // Проверка прав
            if (report.UserId != currentUserId && currentUserRole != "Admin")
                return Forbid();

            if (dto.Quantity.HasValue)
            {
                report.Quantity = dto.Quantity.Value;
                // Пересчитываем стоимость
                report.TotalPrice = report.PricePerUnit * report.Quantity;
            }

            if (dto.Comment != null)
                report.Comment = dto.Comment;

            report.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
