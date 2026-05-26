using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;

namespace SmartBiz_360.Services
{
    public class CrmService
    {
        private readonly AppDbContext _db;

        public CrmService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _db.Customers
                .Include(c => c.AssignedRep)
                .ToListAsync();
        }

        public async Task CreateAsync(Customer customer)
        {
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c != null)
            {
                _db.Customers.Remove(c);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetSalesRepsAsync()
        {
            return await _db.Users
                .Where(u => u.Role == "Manager" || u.Role == "Employee")
                .ToListAsync();
        }

        public async Task CheckAtRiskAsync()
        {
            var customers = await _db.Customers.ToListAsync();
            foreach (var c in customers)
            {
                int days = (int)(DateTime.Now - c.LastContactDate).TotalDays;
                c.IsAtRisk = days > 30;
                if (c.IsAtRisk && c.LifecycleStage != "Churned")
                    c.LifecycleStage = "At Risk";
            }
            await _db.SaveChangesAsync();
        }
    }
}