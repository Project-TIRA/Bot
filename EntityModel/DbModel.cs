using Microsoft.EntityFrameworkCore;

namespace EntityModel
{
    public class DbModel : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }

        public DbModel(DbContextOptions<DbModel> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Phone number should be unique for each organization.
            modelBuilder.Entity<Organization>()
                .HasIndex(o => o.PhoneNumber)
                .IsUnique();
        }
    }
}
