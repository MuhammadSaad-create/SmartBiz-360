using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;


namespace SmartBiz360.Services
{
    public class CampaignService
    {
        private readonly AppDbContext _db;

        public CampaignService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Campaign>> GetAllAsync()
            => await _db.Campaigns.ToListAsync();

        public async Task CreateAsync(Campaign campaign)
        {
            _db.Campaigns.Add(campaign);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Campaign campaign)
        {
            _db.Campaigns.Update(campaign);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _db.Campaigns.FindAsync(id);
            if (c != null)
            {
                _db.Campaigns.Remove(c);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var c = await _db.Campaigns.FindAsync(id);
            if (c != null)
            {
                c.Status = status;
                await _db.SaveChangesAsync();
            }
        }

        public decimal CalculateROI(Campaign c, List<Deal> deals)
        {
            if (c.Budget == 0) return 0;
            var revenue = deals
                .Where(d => d.Stage == "Won" &&
                    d.CreatedAt >= c.StartDate &&
                    d.CreatedAt <= c.EndDate)
                .Sum(d => d.EstimatedValue);
            return ((revenue - c.Budget) / c.Budget) * 100;
        }
    }
}