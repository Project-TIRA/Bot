using EntityModel;
using Microsoft.Bot.Builder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.ApiInterface
{
    public interface IApiInterface
    {
        /// <summary>
        /// Creates a new record of a model.
        /// </summary>
        Task<string> Create<T>(T model) where T : ModelBase;

        /// <summary>
        /// Saves changes to a model.
        /// </summary>
        Task<bool> Update<T>(T model) where T : ModelBase;

        /// <summary>
        /// Gets a user from the turn context.
        /// </summary>
        Task<User> GetUser(ITurnContext turnContext);

        /// <summary>
        /// Gets an organization from the turn context.
        /// </summary>
        Task<Organization> GetOrganization(ITurnContext turnContext, string organizationId);

        /// <summary>
        /// Gets the count of an organization's services from the turn context.
        /// </summary>
        Task<int> GetServiceCount(ITurnContext turnContext, string organizationId);

        /// <summary>
        /// Gets an organization's service by type from the turn context.
        /// </summary>
        Task<Service> GetService<T>(ITurnContext turnContext, string organizationId) where T : ServiceDataBase;

        /// <summary>
        /// Gets all of an organization's services from the turn context.
        /// </summary>
        Task<List<Service>> GetServices(ITurnContext turnContext, string organizationId);

        /// <summary>
        /// Gets the latest shapshot for a service from the turn context.
        /// </summary>
        /// <param name="createdByUser">Optionally pass whether to get the latest data created by the current user</param>
        Task<T> GetLatestServiceData<T>(ITurnContext turnContext, string organizationId, bool createdByUser = false) where T : ServiceDataBase, new();

        /// <summary>
        /// Gets all verified organizations.
        /// </summary>
        Task<List<Organization>> GetVerifiedOrganizations();

        /// <summary>
        /// Gets all users for an organization.
        /// </summary>
        Task<List<User>> GetUsersForOrganization(Organization organization);
    }
}
