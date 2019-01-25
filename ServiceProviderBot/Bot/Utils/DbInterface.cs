using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.EntityFrameworkCore;
using System;
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

        /// <summary>
        /// Saves pending changes to the database.
        /// </summary>
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
            await Save();

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
            await Save();

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

        /// <summary>
        /// Removes incomplete conversation data that has expired.
        /// </summary>
        public async Task<bool> CheckExpiredConversation(ITurnContext context)
        {
            // Expires after 6 hours.
            var expiration = DateTime.UtcNow.AddHours(-6);
            bool didRemove = false;

            // Check for an incomplete organization.
            var organization = await GetOrganization(context);

            if (organization != null &&!organization.IsComplete &&
                organization.DateCreated < expiration)
            {
                this.dbContext.Organizations.Remove(organization);
                didRemove = true;
            }

            // Check for an incomplete snapshot.
            var snapshot = await GetSnapshot(context);

            if (snapshot != null && !snapshot.IsComplete &&
                snapshot.Date < expiration)
            {
                this.dbContext.Snapshots.Remove(snapshot);
                didRemove = true;
            }

            await Save();
            return didRemove;
        }
    }
}
