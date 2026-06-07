namespace SmartBiz360.Models
{
    public class TaskReport
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int ToDoTasks { get; set; }
        public double CompletionRate { get; set; }
        public double OnTimeRate { get; set; }
        public string Grade { get; set; } = string.Empty;      // Excellent / Average / Warning / Serious Warning
        public bool BonusEligible { get; set; }
        public string EmailSentTo { get; set; } = string.Empty;
        public bool EmailSent { get; set; } = false;
    }
}
