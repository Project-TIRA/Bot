using EntityModel;
using EntityModel.Helpers;
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
        Task<string> Create(Model model);

        /// <summary>
        /// Saves changes to a model.
        /// </summary>
        Task<bool> Update(Model model);

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        Task<User> GetUser(string userId);

        /// <summary>
        /// Gets a user from a turn context.
        /// </summary>
        Task<User> GetUser(ITurnContext turnContext);

        /// <summary>
        /// Gets an organization by ID.
        /// </summary>
        Task<Organization> GetOrganization(string organizationId);

        /// <summary>
        /// Gets the count of an organization's services.
        /// </summary>
        Task<int> GetServiceCount(string organizationId);

        /// <summary>
        /// Gets an organization's service by type.
        /// </summary>
        Task<Service> GetService(string organizationId, ServiceType serviceType);

        /// <summary>
        /// Gets all of an organization's services.
        /// </summary>
        Task<List<Service>> GetServices(string organizationId);

        /// <summary>
        /// Gets the latest shapshot for a service.
        /// </summary>
        /// <param name="createdByUserTurnContext">Optionally pass a turn context to get the latest data created by the user</param>
        Task<ServiceData> GetLatestServiceData(string organizationId, ServiceData dataType, ITurnContext createdByUserTurnContext = null);

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
