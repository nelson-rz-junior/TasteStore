using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TasteStore.Models;

namespace TasteStore.DataAccess
{
    // MIGRATIONS
    // dotnet ef migrations add InitialConfiguration --context ApplicationDbContext --startup-project D:\Projects\TasteStore\TasteStore\TasteStore.csproj
    // dotnet ef database update --context ApplicationDbContext --startup-project D:\Projects\TasteStore\TasteStore\TasteStore.csproj

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<FoodType> FoodTypes { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
