using EntityModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.ApiInterface
{
    /// <summary>
    /// API interface for Entity Framework
    /// </summary>
    public class EfInterface : IApiInterface
    {
        private DbModel dbContext { get; }

        public EfInterface(DbModel dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new record of a model.
        /// </summary>
        public async Task<string> Create<T>(T model) where T : ModelBase
        {
            bool success = true;

            if (model is User)
            {
                await this.dbContext.Users.AddAsync(model as User);
            }
            else if (model is Organization)
            {
                await this.dbContext.Organizations.AddAsync(model as Organization);
            }
            else if (model is Organization)
            {
                await this.dbContext.Organizations.AddAsync(model as Organization);
            }
            else if (model is CaseManagementData)
            {
                await this.dbContext.CaseManagementData.AddAsync(model as CaseManagementData);
            }
            else if (model is HousingData)
            {
                await this.dbContext.HousingData.AddAsync(model as HousingData);
            }
            else if (model is JobTrainingData)
            {
                await this.dbContext.JobTrainingData.AddAsync(model as JobTrainingData);
            }
            else if (model is MentalHealthData)
            {
                await this.dbContext.MentalHealthData.AddAsync(model as MentalHealthData);
            }
            else if (model is SubstanceUseData)
            {
                await this.dbContext.SubstanceUseData.AddAsync(model as SubstanceUseData);
            }
            else
            {
                success = false;
            }

            await this.dbContext.SaveChangesAsync();
            return success ? model.ResourceId : string.Empty;
        }

        /// <summary>
        /// Saves changes to a model.
        /// </summary>
        public async Task<bool> Update<T>(T model) where T : ModelBase
        {
            await this.dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a user from a user ID.
        /// </summary>
        public async Task<User> GetUser(string userId)
        {
            var user = await this.dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }

        /// <summary>
        /// Gets an organization from a user ID.
        /// </summary>
        public async Task<Organization> GetOrganization(string userId)
        {
            var user = await GetUser(userId);
            if (user != null)
            {
                return await this.dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == user.OrganizationId);
            }

            return null;
        }

        /// <summary>
        /// Gets the count of an organization's services from a user ID.
        /// </summary>
        public async Task<int> GetServiceCount(string userId)
        {
            Organization organization = await GetOrganization(userId);
            if (organization != null)
            {
                return await this.dbContext.Services.CountAsync(s => s.OrganizationId == organization.Id);
            }

            return 0;
        }

        /// <summary>
        /// Gets an organization's service by type from a user ID.
        /// </summary>
        public async Task<Service> GetService<T>(string userId) where T : ServiceModelBase
        {
            Organization organization = await GetOrganization(userId);
            if (organization != null)
            {
                var serviceType = Helpers.GetServiceType<T>();
                if (serviceType != ServiceType.Invalid)
                {
                    return await this.dbContext.Services.FirstOrDefaultAsync(s => s.Type == (int)serviceType);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the latest shapshot for a service from a user ID.
        /// </summary>
        public async Task<T> GetLatestServiceData<T>(string userId) where T : ServiceModelBase
        {
            var service = await GetService<T>(userId);
            if (service != null)
            {
                var serviceType = Helpers.GetServiceType<T>();
                switch (serviceType)
                {
                    case ServiceType.CaseManagement: return await this.dbContext.CaseManagementData.OrderByDescending(s => s.CreatedOn).FirstOrDefaultAsync() as T;
                    case ServiceType.Housing: return await this.dbContext.HousingData.OrderByDescending(s => s.CreatedOn).FirstOrDefaultAsync() as T;
                    case ServiceType.JobTraining: return await this.dbContext.JobTrainingData.OrderByDescending(s => s.CreatedOn).FirstOrDefaultAsync() as T;
                    case ServiceType.MentalHealth: return await this.dbContext.MentalHealthData.OrderByDescending(s => s.CreatedOn).FirstOrDefaultAsync() as T;
                    case ServiceType.SubstanceUse: return await this.dbContext.SubstanceUseData.OrderByDescending(s => s.CreatedOn).FirstOrDefaultAsync() as T;
                }
            }

            return null;
        }


        /*
        /// <summary>
        /// Removes incomplete conversation data that has expired.
        /// </summary>
        public async Task<bool> CheckExpiredConversation(ITurnContext context, bool forceExpire)
        {
            // Expires after 6 hours.
            var expiration = DateTime.UtcNow.AddHours(-6);
            bool didRemove = false;

            // Check for an incomplete organization.
            var organization = await GetOrganization(context);

            if (organization != null && 
                (forceExpire || !organization.IsComplete && organization.DateCreated < expiration))
            {
                this.dbContext.Organizations.Remove(organization);
                didRemove = true;
            }

            // Check for an incomplete snapshot.
            var snapshot = await GetSnapshot(context);

            if (snapshot != null &&
                (forceExpire || !snapshot.IsComplete && snapshot.Date < expiration))
            {
                this.dbContext.Snapshots.Remove(snapshot);
                didRemove = true;
            }

            await Save();
            return didRemove || forceExpire;
        }
        */
    }
}
