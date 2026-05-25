namespace SmartBiz_360.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = "Active";
        public bool MonitoringConsent { get; set; } = false;
        public DateTime JoinDate { get; set; } = DateTime.Now;
    }
}