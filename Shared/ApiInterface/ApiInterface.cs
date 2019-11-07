using EntityModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

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
        Task<Organization> GetOrganization(ITurnContext turnContext);

        /// <summary>
        /// Gets the count of an organization's services from the turn context.
        /// </summary>
        Task<int> GetServiceCount(ITurnContext turnContext);

        /// <summary>
        /// Gets an organization's service by type from the turn context.
        /// </summary>
        Task<Service> GetService<T>(ITurnContext turnContext) where T : ServiceDataBase;

        /// <summary>
        /// Gets all of an organization's services from the turn context.
        /// </summary>
        Task<List<Service>> GetServices(ITurnContext turnContext);

        /// <summary>
        /// Gets the latest shapshot for a service from the turn context.
        /// </summary>
        /// <param name="createdByUser">Whether or not to get the latest token that was created by the given user</param>
        Task<T> GetLatestServiceData<T>(ITurnContext turnContext, bool createdByUser = false) where T : ServiceDataBase, new();

        /// <summary>
        /// Gets all verified organizations.
        /// </summary>
        Task<List<Organization>> GetVerifiedOrganizations();

        /// <summary>
        /// Gets the list of services from an organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns> List of services </returns>
        Task<List<Service>> GetServicesForOrganization(Organization organization);

        /// <summary>
        /// Gets all users for an organization.
        /// </summary>
        Task<List<User>> GetUsersForOrganization(Organization organization);
    }
}
