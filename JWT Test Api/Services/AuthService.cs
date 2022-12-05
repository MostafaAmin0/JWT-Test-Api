using JWT_Test_Api.Helpers;
using JWT_Test_Api.Models;
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
        private static readonly User _user = new();

        public AuthService(IOptions<JWT> jwt, ApplicationDbContext context)
        {
            _jwt = jwt.Value;
            _context = context;
        }

        public async Task<User?> RegisterAsync(UserDto userDto)
        {
            //if (await _context.users.SingleOrDefaultAsync(u => u.Username == userDto.Username) is not null)
            //{
            //    return null;
            //}

            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            _user.Username = userDto.Username;
            _user.PasswordHash = passwordHash;
            _user.PasswordSalt = passwordSalt;

            //var user = new User
            //{
            //    Username = userDto.Username,
            //    PasswordHash = passwordHash,
            //    PasswordSalt = passwordSalt
            //};

            //await _context.users.AddAsync(user);
            //await _context.SaveChangesAsync();
            //return user;

            return _user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public string? LoginAsync(UserDto userDto)
        {
            if (_user.Username != userDto.Username)
            {
                return null;
            }

            if (!VerifyPasswordHash(userDto.Password, _user.PasswordHash, _user.PasswordSalt))
            {
                return null;
            }

            string token = CreateToken(_user);

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
