using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
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
            else if (model is EmploymentData)
            {
                await this.dbContext.EmploymentData.AddAsync(model as EmploymentData);
            }
            else if (model is MentalHealthData)
            {
                await this.dbContext.MentalHealthData.AddAsync(model as MentalHealthData);
            }
            else if (model is SubstanceUseData)
            {
                await this.dbContext.SubstanceUseData.AddAsync(model as SubstanceUseData);
            }
            else if (model is Feedback)
            {
                await this.dbContext.Feedback.AddAsync(model as Feedback);
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
        /// Gets a user from the turn context.
        /// </summary>
        public async Task<User> GetUser(ITurnContext turnContext)
        {
            var userToken = Helpers.GetUserToken(turnContext);

            switch (turnContext.Activity.ChannelId)
            {
                case Channels.Emulator: return await this.dbContext.Users.FirstOrDefaultAsync(u => u.Name == userToken);
                case Channels.Sms: return await this.dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == userToken);
                default: Debug.Fail("Missing channel type"); return null;
            }
        }

        /// <summary>
        /// Gets an organization from the turn context.
        /// </summary>
        public async Task<Organization> GetOrganization(ITurnContext turnContext, string organizationId)
        {            
            if (!string.IsNullOrEmpty(organizationId))
            {
                return await this.dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId);
            }

            return null;
        }

        /// <summary>
        /// Gets the count of an organization's services from the turn context.
        /// </summary>
        public async Task<int> GetServiceCount(ITurnContext turnContext, string organizationId)
        { 
            if (!string.IsNullOrEmpty(organizationId))
            {
                return await this.dbContext.Services.CountAsync(s => s.OrganizationId == organizationId);
            }

            return 0;
        }

        /// <summary>
        /// Gets an organization's service by type from the turn context.
        /// </summary>
        public async Task<Service> GetService<T>(ITurnContext turnContext, string organizationId) where T : ServiceDataBase
        {
            if (!string.IsNullOrEmpty(organizationId))
            {
                var type = Helpers.GetServiceType<T>();
                if (type != ServiceType.Invalid)
                {
                    return await this.dbContext.Services.FirstOrDefaultAsync(s => s.OrganizationId == organizationId && s.Type == type);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all of an organization's services from the turn context.
        /// </summary>
        public async Task<List<Service>> GetServices(ITurnContext turnContext, string organizationId)
        {
            if (!string.IsNullOrEmpty(organizationId))
            {
                return await this.dbContext.Services.Where(s => s.OrganizationId == organizationId).ToListAsync();
            }

            return new List<Service>();
        }

        /// <summary>
        /// Gets the latest shapshot for a service from the turn context.
        /// </summary>
        /// <param name="createdByUser">Optionally pass whether to get the latest data created by the current user</param>
        public async Task<T> GetLatestServiceData<T>(ITurnContext turnContext, string organizationId, bool createdByUser = false) where T : ServiceDataBase, new()
        {
            var service = await GetService<T>(turnContext, organizationId);
            if (service != null)
            {
                IQueryable<ServiceDataBase> query;

                var type = Helpers.GetServiceType<T>();
                switch (type)
                {
                    case ServiceType.CaseManagement: query = this.dbContext.CaseManagementData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.Housing: query = this.dbContext.HousingData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.Employment: query = this.dbContext.EmploymentData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.MentalHealth: query = this.dbContext.MentalHealthData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.SubstanceUse: query = this.dbContext.SubstanceUseData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    default: return null;
                }

                if (createdByUser)
                {
                    var user = await GetUser(turnContext);
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
    }    
}
