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
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cart → AppUser
            builder.Entity<Cart>()
                .HasOne(c => c.AppUser)
                .WithMany()
                .HasForeignKey(c => c.AppUserId);

            // CartItem → Cart
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            // CartItem → Menu
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Menu)
                .WithMany()
                .HasForeignKey(ci => ci.MenuId);
        }
    }
}
