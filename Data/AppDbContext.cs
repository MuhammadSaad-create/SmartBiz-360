using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Models;
using SmartBiz360.Models;

namespace SmartBiz_360.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskReport> TaskReports { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

}