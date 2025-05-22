using KGTT_Educate.Services.Events.Models;
using Microsoft.EntityFrameworkCore;

namespace KGTT_Educate.Services.Events.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventGroup> EventGroup { get; set; }
        public DbSet<EventUser> EventUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Events");
        }
    }
}
