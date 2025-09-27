using DOAN.Data;
using DOAN.Models;
using DOAN.Respository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DOAN.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null || !VerifyPassword(password, user.Password))
            {
                return null;
            }

            // Update last login
            user.LastLoginAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> RegisterAsync(RegisterViewModel model)
        {
            // Check if email already exists
            if (await EmailExistsAsync(model.Email))
            {
                return null;
            }

            // Check if username already exists
            if (await UsernameExistsAsync(model.Username))
            {
                return null;
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = HashPassword(model.Password),
                FullName = model.FullName,
                Phone = model.Phone,
                Address = model.Address,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}
