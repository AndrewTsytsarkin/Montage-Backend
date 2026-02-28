using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MontageAPI.Data;

namespace WebApplication1.Controllers
{

    public class LoginDto
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }





    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ObjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ObjectsController(AppDbContext context) { _context = context; }
        [HttpGet]
        public async Task<IActionResult> GetObjects()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int userId = int.Parse(userIdClaim);
            IQueryable<ProjectObject> query = _context.ProjectObjects;

            if (roleClaim == "Worker")
            {
                query = query.Where(o => _context.UserObjectAssignments
                    .Any(a => a.ObjectId == o.Id && a.UserId == userId));
            }

            return Ok(await query.ToListAsync());
        }
    }
}
