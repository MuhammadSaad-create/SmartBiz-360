using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;
using SmartBiz360.Data;
using SmartBiz360.Models;

namespace SmartBiz360.Services
{
    public class TaskService
    {
        private readonly AppDbContext _db;

        public TaskService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _db.Tasks
                .Include(t => t.AssignedTo)
                .ToListAsync();
        }

        public async Task CreateAsync(TaskItem task)
        {
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _db.Tasks.Update(task);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, string newStatus)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task != null)
            {
                task.Status = newStatus;
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task != null)
            {
                _db.Tasks.Remove(task);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetUsersAsync()
            => await _db.Users.ToListAsync();
    }
}