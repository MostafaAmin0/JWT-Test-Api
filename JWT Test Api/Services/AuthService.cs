using JWT_Test_Api.Helpers;
using JWT_Test_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace JWT_Test_Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly JWT _jwt;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IOptions<JWT> jwt, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _jwt = jwt.Value;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> RegisterAsync(UserDto userDto)
        {
            var isUserExist = await _context.users.AnyAsync(u => u.Username == userDto.Username);
            if (isUserExist == true)
            {
                return null;
            }

            PasswordUtils.CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task<string?> LoginAsync(UserDto userDto)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.Username == userDto.Username);

            if (user == null)
            {
                return null;
            }

            if (!PasswordUtils.VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            string token = PasswordUtils.CreateToken(user, _jwt);

            return token;
        }

        public string DummyAuthorizationTest()
        {
            string result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
        }
    }
}
