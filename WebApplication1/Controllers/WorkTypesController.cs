using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MontageAPI.Data;

namespace WebApplication1.Controllers
{
    public class WorkTypeDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Subtype { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal PricePerUnit { get; set; }  // ✅ Должно быть!

    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Только авторизованные пользователи
    public class WorkTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkTypesController(AppDbContext context) => _context = context;

        /// <summary>
        /// Получить все виды работ (доступно всем авторизованным)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllWorkTypes(
            [FromQuery] string? type,
            [FromQuery] string? subtype)
        {
            IQueryable<WorkType> query = _context.WorkTypes;

            // Фильтрация по типу (если передан параметр)
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(w => w.Type == type);
            }

            // Фильтрация по подтипу (если передан параметр)
            if (!string.IsNullOrEmpty(subtype))
            {
                query = query.Where(w => w.Subtype == subtype);
            }

            var result = await query
                .OrderBy(w => w.Type)
                .ThenBy(w => w.Subtype)
                .ThenBy(w => w.SortOrder)
                .Select(w => new WorkTypeDto
                {
                    Id = w.Id,
                    Type = w.Type,
                    Subtype = w.Subtype,
                    Name = w.Name,
                    Unit = w.Unit,
                    PricePerUnit = w.PricePerUnit,
                })
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Получить уникальные типы видов работ (для фильтров)
        /// </summary>
        [HttpGet("types")]
        public async Task<IActionResult> GetUniqueTypes()
        {
            var types = await _context.WorkTypes
                .Select(w => w.Type)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return Ok(types);
        }

        /// <summary>
        /// Получить подтипы по типу (для каскадных фильтров)
        /// </summary>
        [HttpGet("types/{typeName}/subtypes")]
        public async Task<IActionResult> GetSubtypesByType(string typeName)
        {
            var subtypes = await _context.WorkTypes
                .Where(w => w.Type == typeName)
                .Select(w => w.Subtype)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            return Ok(subtypes);
        }
    }
}
