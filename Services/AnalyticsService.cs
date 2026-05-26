using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;


namespace SmartBiz360.Services
{
    public class AnalyticsService
    {
        private readonly AppDbContext _db;

        public AnalyticsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> GetTotalEmployeesAsync()
            => await _db.Employees.CountAsync();

        public async Task<int> GetTotalCustomersAsync()
            => await _db.Customers.CountAsync();

        public async Task<int> GetAtRiskCustomersAsync()
            => await _db.Customers.CountAsync(c => c.IsAtRisk);

        public async Task<int> GetNewCustomersAsync()
            => await _db.Customers.CountAsync(c =>
                c.CreatedAt >= DateTime.Now.AddMonths(-1));

        public async Task<int> GetTotalDealsAsync()
            => await _db.Deals.CountAsync();

        public async Task<int> GetOpenDealsAsync()
            => await _db.Deals.CountAsync(d =>
                d.Stage != "Won" && d.Stage != "Lost");

        public async Task<int> GetWonDealsAsync()
            => await _db.Deals.CountAsync(d => d.Stage == "Won");

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var deals = await _db.Deals
                .Where(d => d.Stage == "Won")
                .ToListAsync();
            return deals.Sum(d => d.EstimatedValue);
        }
        public async Task<int> GetTotalTasksAsync()
            => await _db.Tasks.CountAsync();

        public async Task<int> GetCompletedTasksAsync()
            => await _db.Tasks.CountAsync(t => t.Status == "Done");

        public async Task<int> GetOverdueTasksAsync()
            => await _db.Tasks.CountAsync(t =>
                t.DueDate < DateTime.Today && t.Status != "Done");

        public async Task<Dictionary<string, int>> GetDealStagesAsync()
            => await _db.Deals
                .GroupBy(d => d.Stage)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

        public async Task<Dictionary<string, int>> GetTaskStatusesAsync()
            => await _db.Tasks
                .GroupBy(t => t.Status)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
    }
}