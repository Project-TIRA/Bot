using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Utils
{
    public class DbInterface
    {
        private DbModel dbContext { get; }

        public DbInterface(DbModel dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Save()
        {
            await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Creates an organization in the database.
        /// </summary>
        public async Task<Organization> CreateOrganization(ITurnContext context)
        {
            var phoneNumber = context.Activity.From.Id;

            var organization = new Organization();
            organization.PhoneNumber = phoneNumber;
            await this.dbContext.Organizations.AddAsync(organization);
            await this.dbContext.SaveChangesAsync();

            return organization;
        }

        /// <summary>
        /// Gets the current organization from the database.
        /// </summary>
        public async Task<Organization> GetOrganization(ITurnContext context)
        {
            var phoneNumber = context.Activity.From.Id;
            return await this.dbContext.Organizations.FirstOrDefaultAsync(o => o.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// Creates an shapshot in the database.
        /// </summary>
        public async Task<Snapshot> CreateSnapshot(ITurnContext context)
        {
            var phoneNumber = context.Activity.From.Id;
            var organization = await this.dbContext.Organizations.FirstOrDefaultAsync(o => o.PhoneNumber == phoneNumber);

            if (organization == null)
            {
                return null;
            }

            var snapshot = new Snapshot(organization.Id);
            await this.dbContext.Snapshots.AddAsync(snapshot);
            await this.dbContext.SaveChangesAsync();

            return snapshot;
        }

        /// <summary>
        /// Gets the latest snapshot from the database.
        /// </summary>
        public async Task<Snapshot> GetSnapshot(ITurnContext context)
        {
            var phoneNumber = context.Activity.From.Id;
            var organization = await this.dbContext.Organizations.FirstOrDefaultAsync(o => o.PhoneNumber == phoneNumber);

            if (organization == null)
            {
                return null;
            }

            // Get the most recent snapshot.
            return organization.Snapshots.OrderByDescending(s => s.Date).FirstOrDefault();
        }
    }
}
