using Microsoft.EntityFrameworkCore;
using VideoStream.API.Models;

namespace VideoStream.API.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TrainingVideo> TrainingVideos { get; set; } = null!;
    }
}