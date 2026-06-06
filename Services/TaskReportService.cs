using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Data;
using SmartBiz_360.Models;
using SmartBiz360.Data;
using SmartBiz360.Models;

namespace SmartBiz360.Services
{
    public class TaskReportService
    {
        private readonly AppDbContext _db;
        private readonly EmailService _email;

        public TaskReportService(AppDbContext db, EmailService email)
        {
            _db = db;
            _email = email;
        }

        // ── Grade thresholds ────────────────────────────────────────────
        // Excellent  : completion ≥ 80% AND onTime ≥ 70%
        // Average    : completion ≥ 60% AND onTime ≥ 50%
        // Warning    : completion ≥ 40%
        // Serious Warning: below that
        private static (string Grade, bool Bonus) DetermineGrade(
            double completionRate, double onTimeRate)
        {
            if (completionRate >= 80 && onTimeRate >= 70)
                return ("Excellent", true);
            if (completionRate >= 60 && onTimeRate >= 50)
                return ("Average", false);
            if (completionRate >= 40)
                return ("Warning", false);
            return ("Serious Warning", false);
        }

        public async Task<List<TaskReport>> GetAllReportsAsync()
        {
            return await _db.TaskReports
                .Include(r => r.User)
                .Include(r => r.Employee)
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();
        }

        public async Task<List<TaskReport>> GetReportsByUserAsync(int userId)
        {
            return await _db.TaskReports
                .Include(r => r.User)
                .Include(r => r.Employee)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();
        }

        public async Task<TaskReport?> GenerateReportAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return null;

            var employee = await _db.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null) return null;

            var now = DateTime.Now;
            var tasks = await _db.Tasks
                .Where(t => t.AssignedToId == userId)
                .ToListAsync();

            if (tasks.Count == 0) return null;

            int total = tasks.Count;
            int completed = tasks.Count(t => t.Status == "Done");
            int inProgress = tasks.Count(t => t.Status == "In Progress");
            int toDo = tasks.Count(t => t.Status == "To Do");
            int overdue = tasks.Count(t =>
                t.Status != "Done" && t.DueDate < now);

            double completionRate = total > 0
                ? Math.Round((double)completed / total * 100, 1) : 0;

            // on-time = completed tasks whose DueDate was in the future
            int completedOnTime = tasks.Count(t =>
                t.Status == "Done" && t.DueDate >= t.CreatedAt);
            double onTimeRate = completed > 0
                ? Math.Round((double)completedOnTime / completed * 100, 1) : 0;

            var (grade, bonus) = DetermineGrade(completionRate, onTimeRate);

            var report = new TaskReport
            {
                UserId = userId,
                EmployeeId = employee.Id,
                GeneratedAt = now,
                TotalTasks = total,
                CompletedTasks = completed,
                InProgressTasks = inProgress,
                ToDoTasks = toDo,
                OverdueTasks = overdue,
                CompletionRate = completionRate,
                OnTimeRate = onTimeRate,
                Grade = grade,
                BonusEligible = bonus,
                EmailSentTo = user.Email
            };

            _db.TaskReports.Add(report);
            await _db.SaveChangesAsync();

            return report;
        }

        public async Task<bool> SendReportEmailDirectAsync(
            TaskReport report, User user, EmailService emailSvc)
        {
            var gradeColor = report.Grade switch
            {
                "Excellent" => "#16a34a",
                "Average" => "#ca8a04",
                "Warning" => "#ea580c",
                "Serious Warning" => "#dc2626",
                _ => "#6b7280"
            };

            var bonusBadge = report.BonusEligible
                ? "✅ Bonus Eligible"
                : "❌ Not Bonus Eligible";

            await emailSvc.SendTaskReportEmailAsync(
                user.Email, user.FullName,
                report.Grade, report.CompletedTasks, report.TotalTasks,
                report.CompletionRate, report.OnTimeRate,
                report.OverdueTasks, report.BonusEligible,
                report.GeneratedAt);

            return true;
        }
    }
}
