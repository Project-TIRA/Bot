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
                    return await this.dbContext.Services.FirstOrDefaultAsync(s => s.OrganizationId == organization.Id && s.Type == type);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all of an organization's services from a user token.
        /// </summary>
        public async Task<List<Service>> GetServices(string userToken)
        {
            Organization organization = await GetOrganization(userToken);
            if (organization != null)
            {
                return await this.dbContext.Services.Where(s => s.OrganizationId == organization.Id).ToListAsync();
            }

            return new List<Service>();
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
                    case ServiceType.CaseManagement: query = this.dbContext.CaseManagementData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.Housing: query = this.dbContext.HousingData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.JobTraining: query = this.dbContext.JobTrainingData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.MentalHealth: query = this.dbContext.MentalHealthData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.SubstanceUse: query = this.dbContext.SubstanceUseData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
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

        /// <summary>
        /// Returns whether or not the conversation is expired.
        /// </summary>
        public async Task<bool> IsUpdateExpired(string userToken)
        {
            // Expires after 12 hours.
            var expiration = DateTime.UtcNow.AddHours(-12);
            bool isExpired =  await IsServiceDataExpired<CaseManagementData>(userToken);

            if (!isExpired)
            {
                isExpired |= await IsServiceDataExpired<HousingData>(userToken);
            }

            if (!isExpired)
            {
                isExpired |= await IsServiceDataExpired<JobTrainingData>(userToken);
            }

            if (!isExpired)
            {
                isExpired |= await IsServiceDataExpired<MentalHealthData>(userToken);
            }

            if (!isExpired)
            {
                isExpired |= await IsServiceDataExpired<SubstanceUseData>(userToken);
            }

            return isExpired;
        }

        private async Task<bool> IsServiceDataExpired<T>(string userToken) where T : ServiceModelBase, new()
        {
            var expiration = DateTime.UtcNow.AddHours(-Phrases.Reset.TimeoutHours);
            var data = await GetLatestServiceData<T>(userToken, createdByUser: true);
            return data != null && !data.IsComplete && data.CreatedOn < expiration;
        }
    }    
}
