using System.Data.Entity;

namespace anipet.Models
{
    public class DBContext : DbContext
    {
        public DbSet<Product> Prodcts { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<FoodSource> FoodSources { get; set; }

        public DbSet<Purchase> Purchases { get; set; }
    }
}
