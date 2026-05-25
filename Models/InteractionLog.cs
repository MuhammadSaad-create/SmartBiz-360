namespace SmartBiz_360.Models
{
    public class InteractionLog
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Type { get; set; } = "Call";
        public string Notes { get; set; } = string.Empty;
        public DateTime InteractionDate { get; set; } = DateTime.Now;
    }
}