using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;


namespace SmartBiz360.Services
{
    public class ProductivityService
    {
        private readonly AppDbContext _db;

        public ProductivityService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProductivityScore>> GetAllAsync()
            => await _db.ProductivityScores
                .Include(p => p.Employee)
                .ThenInclude(e => e.User)
                .ToListAsync();

        public async Task<List<ProductivityScore>> GetByEmployeeAsync(
            int employeeId)
            => await _db.ProductivityScores
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();

        public async Task<ProductivityScore?> GetTodayScoreAsync(
            int employeeId)
            => await _db.ProductivityScores
                .FirstOrDefaultAsync(p =>
                    p.EmployeeId == employeeId &&
                    p.Date == DateTime.Today);

        // Core scoring engine — 4 weighted components
        public async Task CalculateAndSaveAsync(int employeeId)
        {
            var employee = await _db.Employees
                .FindAsync(employeeId);
            if (employee == null) return;

            // Get task completion rate for this employee
            var totalTasks = await _db.Tasks
                .CountAsync(t => t.AssignedToId == employee.UserId);
            var completedTasks = await _db.Tasks
                .CountAsync(t => t.AssignedToId == employee.UserId
                    && t.Status == "Done");

            double taskRate = totalTasks == 0 ? 0.5 :
                (double)completedTasks / totalTasks;

            // Simulated values for other components
            // In production these come from monitoring agent
            var rng = new Random(employeeId + DateTime.Today.DayOfYear);
            double activeTimeRatio = 0.6 + rng.NextDouble() * 0.35;
            double productiveAppUsage = 0.5 + rng.NextDouble() * 0.4;
            double keyboardIntensity = 0.4 + rng.NextDouble() * 0.5;

            // Weighted formula from proposal
            double finalScore =
                (activeTimeRatio * 35) +
                (productiveAppUsage * 30) +
                (keyboardIntensity * 20) +
                (taskRate * 15);

            finalScore = Math.Min(100, Math.Max(0, finalScore));

            // Check if score exists for today
            var existing = await GetTodayScoreAsync(employeeId);
            if (existing != null)
            {
                existing.ActiveTimeRatio = activeTimeRatio;
                existing.ProductiveAppUsage = productiveAppUsage;
                existing.KeyboardIntensity = keyboardIntensity;
                existing.TaskCompletionRate = taskRate;
                existing.FinalScore = finalScore +
                    existing.ManualAdjustment;
                _db.ProductivityScores.Update(existing);
            }
            else
            {
                var score = new ProductivityScore
                {
                    EmployeeId = employeeId,
                    Date = DateTime.Today,
                    ActiveTimeRatio = activeTimeRatio,
                    ProductiveAppUsage = productiveAppUsage,
                    KeyboardIntensity = keyboardIntensity,
                    TaskCompletionRate = taskRate,
                    FinalScore = finalScore
                };
                _db.ProductivityScores.Add(score);
            }

            await _db.SaveChangesAsync();
        }

        public async Task CalculateAllEmployeesAsync()
        {
            var employees = await _db.Employees.ToListAsync();
            foreach (var emp in employees)
                await CalculateAndSaveAsync(emp.Id);
        }

        public async Task ApplyManualAdjustmentAsync(
            int scoreId, int adjustment, string note)
        {
            if (adjustment < -10 || adjustment > 10) return;

            var score = await _db.ProductivityScores
                .FindAsync(scoreId);
            if (score == null) return;

            score.ManualAdjustment = adjustment;
            score.AdjustmentNote = note;
            score.FinalScore = Math.Min(100,
                Math.Max(0, score.FinalScore +
                    adjustment));

            await _db.SaveChangesAsync();
        }

        public async Task<double> GetTeamAverageAsync()
        {
            var scores = await _db.ProductivityScores
                .Where(p => p.Date == DateTime.Today)
                .ToListAsync();
            return scores.Any() ?
                scores.Average(p => p.FinalScore) : 0;
        }
    }
}