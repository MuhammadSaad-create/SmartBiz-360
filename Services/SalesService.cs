using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;

namespace SmartBiz360.Services
{
    public class SalesService
    {
        private readonly AppDbContext _db;
        private readonly EmailService _email;

        public SalesService(AppDbContext db, EmailService email)
        {
            _db = db;
            _email = email;
        }

        public async Task<List<Deal>> GetAllAsync()
        {
            return await _db.Deals
                .Include(d => d.Owner)
                .Include(d => d.Customer)
                .ToListAsync();
        }

        public async Task CreateAsync(Deal deal)
        {
            _db.Deals.Add(deal);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Deal deal)
        {
            _db.Deals.Update(deal);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var deal = await _db.Deals.FindAsync(id);
            if (deal != null)
            {
                _db.Deals.Remove(deal);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateStageAsync(int id, string newStage)
        {
            var deal = await _db.Deals
                .Include(d => d.Owner)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (deal != null)
            {
                deal.Stage = newStage;
                await _db.SaveChangesAsync();

                if (deal.Owner != null)
                {
                    await _email.SendDealUpdateAsync(
                        deal.Owner.Email,
                        deal.Owner.FullName,
                        deal.Title,
                        newStage);
                }
            }
        }
        public async Task<List<Customer>> GetCustomersAsync()
            => await _db.Customers.ToListAsync();

        public async Task<List<User>> GetUsersAsync()
            => await _db.Users.ToListAsync();

        public decimal GetForecastedRevenue(List<Deal> deals)
        {
            var weights = new Dictionary<string, double>
            {
                { "New Lead", 0.10 },
                { "Qualified", 0.25 },
                { "Proposal Sent", 0.50 },
                { "Negotiation", 0.75 },
                { "Won", 1.0 },
                { "Lost", 0.0 }
            };

            return deals.Sum(d =>
                d.EstimatedValue *
                (decimal)(weights.ContainsKey(d.Stage) ?
                    weights[d.Stage] : 0));
        }
    }
}