//Productivity
namespace SmartBiz_360.Models
{
    public class ProductivityScore
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public DateTime Date { get; set; } = DateTime.Today;
        public double ActiveTimeRatio { get; set; }
        public double ProductiveAppUsage { get; set; }
        public double KeyboardIntensity { get; set; }
        public double TaskCompletionRate { get; set; }
        public double FinalScore { get; set; }
        public int ManualAdjustment { get; set; } = 0;
        public string AdjustmentNote { get; set; } = string.Empty;
    }
}