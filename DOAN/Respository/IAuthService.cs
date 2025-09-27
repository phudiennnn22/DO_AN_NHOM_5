using DOAN.Models;

namespace DOAN.Respository
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<User?> RegisterAsync(RegisterViewModel model);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task UpdateLastLoginAsync(int userId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
