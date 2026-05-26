//This is DB
using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Models;

namespace SmartBiz_360.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<InteractionLog> InteractionLogs { get; set; }
        public DbSet<ProductivityScore> ProductivityScores { get; set; }
    }
}