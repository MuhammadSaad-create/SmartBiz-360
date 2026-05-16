using Microsoft.EntityFrameworkCore;
using SmartBiz_360.Models;

namespace SmartBiz_360.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}