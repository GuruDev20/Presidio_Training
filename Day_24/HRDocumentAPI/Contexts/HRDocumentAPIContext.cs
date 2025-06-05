using HRDocumentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HRDocumentAPI.Contexts
{
    public class HRDocumentAPIContext : DbContext
    {
        public HRDocumentAPIContext(DbContextOptions<HRDocumentAPIContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasOne(d => d.UploadedBy)
                .WithMany(u=> u.Documents)
                .HasForeignKey(d => d.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}