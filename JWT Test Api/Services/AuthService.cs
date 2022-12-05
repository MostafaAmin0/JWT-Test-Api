using JWT_Test_Api.Helpers;
using JWT_Test_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JWT_Test_Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly JWT _jwt;
        private readonly ApplicationDbContext _context;
        //private static readonly User _user = new();

        public AuthService(IOptions<JWT> jwt, ApplicationDbContext context)
        {
            _jwt = jwt.Value;
            _context = context;
        }

        public async Task<User?> RegisterAsync(UserDto userDto)
        {
            var isUserExist = await _context.users.AnyAsync(u => u.Username == userDto.Username);
            if (isUserExist == true)
            {
                return null;
            }

            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

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

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public async Task<string?> LoginAsync(UserDto userDto)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.Username == userDto.Username);

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            string token = CreateToken(user);

            return token;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwt.Key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }


        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
