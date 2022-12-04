using JWT_Test_Api.Helpers;
using JWT_Test_Api.Models;
using JWT_Test_Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JWT_Test_Api.Controllers
{
    public class AuthController : Controller
    {

        private static readonly User _user = new();
        private readonly JWT _jWT;
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, IOptions<JWT> jWT)
        {
            _jWT = jWT.Value;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = _authService.RegisterAsync(request);

            if (user == null)
            {
                return BadRequest("User Found");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var token = _authService.LoginAsync(request);
            if (token == null)
            {
                return BadRequest("Something Wrong");
            }

            return Ok(token);
        }
    }
}
