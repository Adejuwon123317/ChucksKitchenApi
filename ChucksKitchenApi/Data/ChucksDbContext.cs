using ChucksKitchenApi.Entity;
using Microsoft.EntityFrameworkCore;

namespace ChucksKitchenApi.Data
{
    public class ChucksDbContext : DbContext
    {
        public ChucksDbContext(DbContextOptions<ChucksDbContext> options): base(options)
        {
        }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
