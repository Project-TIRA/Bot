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
        public async Task<string> Create(Model model)
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
        public async Task<bool> Update(Model model)
        {
            await this.dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        public async Task<User> GetUser(string userId)
        {
            return await this.dbContext.Users.FindAsync(userId);
        }

        /// <summary>
        /// Gets a user from a turn context.
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
        /// Gets an organization by ID.
        /// </summary>
        public async Task<Organization> GetOrganization(string organizationId)
        {            
            if (!string.IsNullOrEmpty(organizationId))
            {
                return await this.dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId);
            }

            return null;
        }

        /// <summary>
        /// Gets the count of an organization's services.
        /// </summary>
        public async Task<int> GetServiceCount(string organizationId)
        { 
            if (!string.IsNullOrEmpty(organizationId))
            {
                return await this.dbContext.Services.CountAsync(s => s.OrganizationId == organizationId);
            }

            return 0;
        }

        /// <summary>
        /// Gets an organization's service by type.
        /// </summary>
        public async Task<Service> GetService(string organizationId, ServiceType serviceType)
        {
            if (!string.IsNullOrEmpty(organizationId))
            {
                if (serviceType != ServiceType.Invalid)
                {
                    return await this.dbContext.Services.FirstOrDefaultAsync(s => s.OrganizationId == organizationId && s.Type == serviceType);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all of an organization's services.
        /// </summary>
        public async Task<List<Service>> GetServices(string organizationId)
        {
            if (!string.IsNullOrEmpty(organizationId))
            {
                return await this.dbContext.Services.Where(s => s.OrganizationId == organizationId).ToListAsync();
            }

            return new List<Service>();
        }

        /// <summary>
        /// Gets the latest shapshot for a service.
        /// </summary>
        /// <param name="createdByUser">Optionally pass a turn context to get the latest data created by the user</param>
        public async Task<ServiceData> GetLatestServiceData(string organizationId, ServiceData dataType, ITurnContext createdByUserTurnContext = null)
        {
            var service = await GetService(organizationId, dataType.ServiceType());
            if (service != null)
            {
                IQueryable<ServiceData> query;

                switch (dataType.ServiceType())
                {
                    // TODO: can this be abstracted to where the type has the table name?
                    case ServiceType.CaseManagement: query = this.dbContext.CaseManagementData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.Housing: query = this.dbContext.HousingData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.Employment: query = this.dbContext.EmploymentData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.MentalHealth: query = this.dbContext.MentalHealthData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    case ServiceType.SubstanceUse: query = this.dbContext.SubstanceUseData.Where(s => s.ServiceId == service.Id).OrderByDescending(s => s.CreatedOn); break;
                    default: return null;
                }

                if (createdByUserTurnContext != null)
                {
                    var user = await GetUser(createdByUserTurnContext);
                    query = query.Where(s => s.CreatedById == user.Id);
                }

                return await query.FirstOrDefaultAsync();
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
