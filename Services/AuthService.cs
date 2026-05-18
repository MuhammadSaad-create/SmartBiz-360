using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;

namespace SmartBiz_360.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;

        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> RegisterAsync(string fullName, string email,
            string password, string role = "Employee")
        {
            bool exists = await _db.Users.AnyAsync(u => u.Email == email);
            if (exists) return false;

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null) return null;

            bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return passwordValid ? user : null;
        }
    }
}