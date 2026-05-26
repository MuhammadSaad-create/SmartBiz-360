//TaskItem
namespace SmartBiz_360.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "To Do";
        public DateTime DueDate { get; set; }
        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal ActualHours { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}