using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;


namespace SmartBiz_360.Services
{
    public class EmployeeService
    {
        private readonly AppDbContext _db;

        public EmployeeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _db.Employees
                .Include(e => e.User)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _db.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> CreateAsync(Employee employee, User user)
        {
            bool exists = await _db.Users
                .AnyAsync(u => u.Email == user.Email);
            if (exists) return false;

            user.PasswordHash = BCrypt.Net.BCrypt
                .HashPassword("Employee@123");
            user.Role = "Employee";

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            employee.UserId = user.Id;
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateAsync(Employee employee)
        {
            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp != null)
            {
                _db.Employees.Remove(emp);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _db.Employees.CountAsync();
        }
    }
}