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
        /// Gets a user from a turn context.
        /// </summary>
        Task<User> GetUser(ITurnContext turnContext);

        /// <summary>
        /// Gets an organization.
        /// </summary>
        Task<Organization> GetOrganization(string organizationId);

        /// <summary>
        /// Gets the count of an organization's services.
        /// </summary>
        Task<int> GetServiceCount(string organizationId);

        /// <summary>
        /// Gets an organization's service by type.
        /// </summary>
        Task<Service> GetService<T>(string organizationId) where T : ServiceDataBase;

        /// <summary>
        /// Gets all of an organization's services.
        /// </summary>
        Task<List<Service>> GetServices(string organizationId);

        /// <summary>
        /// Gets the latest shapshot for a service from the turn context.
        /// </summary>
        /// <param name="createdByUserTurnContext">Optionally pass a turn context to get the latest data created by the user</param>
        Task<T> GetLatestServiceData<T>(string organizationId, ITurnContext createdByUserTurnContext = null) where T : ServiceDataBase, new();

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
