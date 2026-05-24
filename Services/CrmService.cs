using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;


namespace SmartBiz360.Services
{
    public class CrmService
    {
        private readonly AppDbContext _db;
        private readonly EmailService _email;

        public CrmService(AppDbContext db, EmailService email)
        {
            _db = db;
            _email = email;
        }
        public async Task<List<Customer>> GetAllAsync()
        {
            return await _db.Customers
                .Include(c => c.AssignedRep)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _db.Customers
                .Include(c => c.AssignedRep)
                .FirstOrDefaultAsync(c => c.Id == id);
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
            var customer = await _db.Customers.FindAsync(id);
            if (customer != null)
            {
                _db.Customers.Remove(customer);
                await _db.SaveChangesAsync();
            }
        }

        public async Task CheckAtRiskAsync()
        {
            var customers = await _db.Customers
                .Include(c => c.AssignedRep)
                .ToListAsync();

            foreach (var c in customers)
            {
                int days = (int)(DateTime.Now - c.LastContactDate).TotalDays;
                bool atRisk = days > 30;

                if (atRisk && !c.IsAtRisk && c.AssignedRep != null)
                {
                    await _email.SendAtRiskAlertAsync(
                        c.AssignedRep.Email,
                        c.AssignedRep.FullName,
                        c.CompanyName,
                        days);
                }

                c.IsAtRisk = atRisk;
                if (atRisk && c.LifecycleStage != "Churned")
                    c.LifecycleStage = "At Risk";
            }
            await _db.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync()
            => await _db.Customers.CountAsync();

        public async Task<int> GetAtRiskCountAsync()
            => await _db.Customers.CountAsync(c => c.IsAtRisk);

        public async Task<List<User>> GetSalesRepsAsync()
            => await _db.Users
                .Where(u => u.Role == "Manager" || u.Role == "Employee")
                .ToListAsync();
    }
}