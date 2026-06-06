using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;
using SmartBiz360.Data;
using SmartBiz360.Models;

namespace SmartBiz360.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly EmailService _email;

        public AuthService(AppDbContext db, EmailService email)
        {
            _db = db;
            _email = email;
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
            await _email.SendWelcomeEmailAsync(email, fullName, password);
            return true;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null) return null;

            bool passwordValid = BCrypt.Net.BCrypt
                .Verify(password, user.PasswordHash);

            return passwordValid ? user : null;
        }

        // ── Forgot / Reset Password ──────────────────────────────────────

        /// <summary>
        /// Generates a secure reset token and emails the link.
        /// Always returns true even if email is not found (prevents enumeration).
        /// </summary>
        public async Task<bool> SendPasswordResetAsync(string email, string baseUrl)
        {
            Console.WriteLine($"[RESET] Attempting password reset for: {email}");

            // Case-insensitive email match
            var user = await _db.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == email.ToLower() && u.IsActive);

            if (user == null)
            {
                Console.WriteLine($"[RESET] No active user found for: {email}");
                return true; // silent — don't reveal email existence
            }

            Console.WriteLine($"[RESET] Found user: {user.FullName} ({user.Email})");

            // Invalidate any previous unused tokens for this user
            var old = await _db.PasswordResetTokens
                .Where(t => t.UserId == user.Id && !t.IsUsed)
                .ToListAsync();
            _db.PasswordResetTokens.RemoveRange(old);

            // Create new token — valid for 1 hour
            var rawToken = Convert.ToBase64String(
                System.Security.Cryptography.RandomNumberGenerator.GetBytes(48));
            var urlToken = rawToken
                .Replace("+", "-").Replace("/", "_").Replace("=", "");

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = urlToken,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _db.PasswordResetTokens.Add(resetToken);
            await _db.SaveChangesAsync();

            Console.WriteLine($"[RESET] Token saved. Sending email to: {user.Email}");
            Console.WriteLine($"[RESET] Reset link base URL: {baseUrl}");

            await _email.SendPasswordResetEmailAsync(
                user.Email, user.FullName, urlToken, baseUrl);

            Console.WriteLine($"[RESET] Done.");
            return true;
        }

        /// <summary>
        /// Checks whether a token exists, is not expired, and has not been used.
        /// </summary>
        public async Task<bool> IsResetTokenValidAsync(string token)
        {
            return await _db.PasswordResetTokens.AnyAsync(t =>
                t.Token == token &&
                !t.IsUsed &&
                t.ExpiresAt > DateTime.Now);
        }

        /// <summary>
        /// Resets the user's password if the token is valid.
        /// Marks the token as used afterwards.
        /// </summary>
        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var resetToken = await _db.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.Token == token &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.Now);

            if (resetToken == null) return false;

            resetToken.User.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(newPassword);

            resetToken.IsUsed = true;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
