using DOAN.Models;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(AppDbContext context)
        {
            // Check if admin already exists
            var adminExists = await context.Users.AnyAsync(u => u.Email == "admin@store.com");
            if (adminExists) return;

            // Create admin user
            var admin = new User
            {
                Username = "admin",
                Email = "admin@store.com",
                Password = HashPassword("admin123"), // Default password
                FullName = "Administrator",
                Phone = "0123456789",
                Address = "Admin Office",
                IsActive = true,
                Role = UserRole.Admin,
                CreatedAt = DateTime.Now
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
