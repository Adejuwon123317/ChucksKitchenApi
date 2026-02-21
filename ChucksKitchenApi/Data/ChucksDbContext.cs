using ChucksKitchenApi.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChucksKitchenApi.Data
{
    public class ChucksDbContext : IdentityDbContext<AppUser>
    {
        public ChucksDbContext(DbContextOptions<ChucksDbContext> options): base(options)
        {

        }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
