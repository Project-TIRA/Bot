using Microsoft.EntityFrameworkCore;

namespace EntityModel
{
    public class DbModel : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                /*
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DbCoreConnectionString");
                */

                // TODO: Get this from config.
                optionsBuilder.UseSqlServer("data source=(LocalDb)\\MSSQLLocalDB;initial catalog=BotEntityModel;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
            }
        }
    }
}
