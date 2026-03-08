using Microsoft.EntityFrameworkCore;
using Novskiy.Domain.Entities;

namespace Novskiy.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Dish> Dishes { get; set; }
}
