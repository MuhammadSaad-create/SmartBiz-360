namespace SmartBiz_360.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LifecycleStage { get; set; } = "Lead";
        public string ValueTier { get; set; } = "Medium";
        public int? AssignedRepId { get; set; }
        public User? AssignedRep { get; set; }
        public DateTime LastContactDate { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsAtRisk { get; set; } = false;
    }
}