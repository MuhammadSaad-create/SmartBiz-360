//Deals
namespace SmartBiz_360.Models
{
    public class Deal
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal EstimatedValue { get; set; }
        public string Stage { get; set; } = "New Lead";
        public DateTime ExpectedCloseDate { get; set; }
        public int? OwnerId { get; set; }
        public User? Owner { get; set; }
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}