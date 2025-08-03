using Microsoft.EntityFrameworkCore;
using MigrationApp.Models;

namespace MigrationApp.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<News> News { get; set; }
        // public DbSet<Model> Models { get; set; }
        public DbSet<ContactU> ContactUs { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<News>().ToTable("News");
            // modelBuilder.Entity<Model>().ToTable("Models");
            modelBuilder.Entity<ContactU>().ToTable("ContactUs");
            modelBuilder.Entity<Color>().ToTable("Colors");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Cart>().ToTable("Carts");
        }
    }
}