using JWT_Test_Api.Models;
using JWT_Test_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Test_Api.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            var user = await _authService.RegisterAsync(request);

            if (user == null)
            {
                return BadRequest("Something Wrong");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            var token = await _authService.LoginAsync(request);

            if (token == null)
            {
                return BadRequest("Something Wrong");
            }

            return Ok(token);
        }
    }
}
