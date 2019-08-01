using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace EntityModel
{
    public class DbModel : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<CaseManagementData> CaseManagementData { get; set; }
        public DbSet<HousingData> HousingData { get; set; }
        public DbSet<JobTrainingData> JobTrainingData { get; set; }
        public DbSet<MentalHealthData> MentalHealthData { get; set; }
        public DbSet<SubstanceUseData> SubstanceUseData { get; set; }

        public DbModel(DbContextOptions<DbModel> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Phone number should be unique for each user.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();
        }
    }

    public class DbModelFactory : IDesignTimeDbContextFactory<DbModel>
    {
        /// <summary>
        /// Provides a way for EF Core Tools to create a DbContext instance at design time (i.e. running migrations).
        /// See https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
        /// </summary>
        public DbModel CreateDbContext(string[] args)
        {
            // Only used by EF Core Tools, so okay to hardcode to local DB.
            return Create("data source=(LocalDb)\\MSSQLLocalDB;initial catalog=OrganizationModel;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
        }

        public static DbModel Create(string connectionString)
        {
            return new DbModel(new DbContextOptionsBuilder<DbModel>()
               .UseSqlServer(connectionString)
               .Options);
        }

        public static DbModel CreateInMemory()
        {
            return new DbModel(new DbContextOptionsBuilder<DbModel>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options);
        }
    }
}
