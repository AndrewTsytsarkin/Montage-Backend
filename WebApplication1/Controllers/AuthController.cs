using Microsoft.AspNetCore.Mvc;
using MontageAPI.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto.Login, dto.Password);
            if (token == null)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            return Ok(new { Token = token });
        }
    }
}
