using KGTT_Educate.Services.Account.Models;
using Microsoft.EntityFrameworkCore;

namespace KGTT_Educate.Services.Account.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<UserRole> UserRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Users");
        }
    }
}
