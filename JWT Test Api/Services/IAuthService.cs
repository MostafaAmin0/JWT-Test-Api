using JWT_Test_Api.Models;

namespace JWT_Test_Api.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto userDto);
        String? LoginAsync(UserDto userDto);
    }
}
