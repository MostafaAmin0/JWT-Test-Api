using JWT_Test_Api.Models;
using JWT_Test_Api.Services;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Register([FromBody] UserDto request)
        {
            var user = await _authService.RegisterAsync(request);

            if (user == null)
            {
                return BadRequest("Something Wrong");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto request)
        {
            var token = await _authService.LoginAsync(request);

            if (token == null)
            {
                return BadRequest("Something Wrong");
            }

            return Ok(token);
        }

        [HttpGet("DummyAuthorization")]
        //[Authorize(Roles = "Student")]
        [Authorize(Roles = "Admin")]
        public IActionResult DummyAuthorizationTest()
        {
            /// to get user name of authenticated user
            /// this way is not best practice && accessible in controllers only 
            //string userName = User.Identity.Name;

            string result = _authService.DummyAuthorizationTest();

            return Ok(result);
        }
    }
}
