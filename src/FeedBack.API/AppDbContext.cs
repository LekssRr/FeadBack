using Microsoft.EntityFrameworkCore;
using FeadBack.Models;
namespace FeadBack
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<FeadBack.Models.FeadBack> FeadBacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeadBack.Models.FeadBack>()
                .Property(f => f.Id)
                .UseIdentityColumn();
        }
    }
}