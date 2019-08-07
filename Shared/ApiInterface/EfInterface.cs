using EntityModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
            if (model is User)
            {
                await this.dbContext.Users.AddAsync(model as User);
            }
            else if (model is Organization)
            {
                await this.dbContext.Organizations.AddAsync(model as Organization);
            }
            else if (model is Service)
            {
                await this.dbContext.Services.AddAsync(model as Service);
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
                Debug.Assert(false, "Add the new type");
                return string.Empty;
            }

            await this.dbContext.SaveChangesAsync();
            return model.Id;
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
        /// Gets a user from a user token.
        /// </summary>
        public async Task<User> GetUser(string userToken)
        {
            return await this.dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == userToken);
        }

        /// <summary>
        /// Gets an organization from a user token.
        /// </summary>
        public async Task<Organization> GetOrganization(string userToken)
        {
            var user = await GetUser(userToken);
            if (user != null)
            {
                return await this.dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == user.OrganizationId);
            }

            return null;
        }

        /// <summary>
        /// Gets the count of an organization's services from a user token.
        /// </summary>
        public async Task<int> GetServiceCount(string userToken)
        {
            Organization organization = await GetOrganization(userToken);
            if (organization != null)
            {
                return await this.dbContext.Services.CountAsync(s => s.OrganizationId == organization.Id);
            }

            return 0;
        }

        /// <summary>
        /// Gets an organization's service by type from a user token.
        /// </summary>
        public async Task<Service> GetService<T>(string userToken) where T : ServiceModelBase
        {
            Organization organization = await GetOrganization(userToken);
            if (organization != null)
            {
                var type = Helpers.GetServiceType<T>();
                if (type != ServiceType.Invalid)
                {
                    return await this.dbContext.Services.FirstOrDefaultAsync(s => s.Type == (int)type);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the latest shapshot for a service from a user token.
        /// </summary>
        /// <param name="createdByUser">Whether or not to get the latest token that was created by the given user</param>
        public async Task<T> GetLatestServiceData<T>(string userToken, bool createdByUser) where T : ServiceModelBase, new()
        {
            var service = await GetService<T>(userToken);
            if (service != null)
            {
                IQueryable<ServiceModelBase> query;

                var type = Helpers.GetServiceType<T>();
                switch (type)
                {
                    case ServiceType.CaseManagement: query = this.dbContext.CaseManagementData.OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.Housing: query = this.dbContext.HousingData.OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.JobTraining: query = this.dbContext.JobTrainingData.OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.MentalHealth: query = this.dbContext.MentalHealthData.OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.SubstanceUse: query = this.dbContext.SubstanceUseData.OrderByDescending(s => s.CreatedOn); break;
                    default: return null;
                }

                if (createdByUser)
                {
                    var user = await GetUser(userToken);
                    query = query.Where(s => s.CreatedById == user.Id);
                }

                return await query.FirstOrDefaultAsync() as T;
            }

            return null;
        }

        /// <summary>
        /// Gets all verified organizations.
        /// </summary>
        public async Task<List<Organization>> GetVerifiedOrganizations()
        {
            return await this.dbContext.Organizations.Where(o => o.IsVerified).ToListAsync();
        }

        /// <summary>
        /// Gets all users for an organization.
        /// </summary>
        public async Task<List<User>> GetUsersForOrganization(Organization organization)
        {
            return await this.dbContext.Users.Where(u => u.OrganizationId == organization.Id).ToListAsync();
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
