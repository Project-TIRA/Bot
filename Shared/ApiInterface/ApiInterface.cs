using EntityModel;
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
        /// Gets a user from a user token.
        /// </summary>
        Task<User> GetUser(string userToken);

        /// <summary>
        /// Gets an organization from a user token.
        /// </summary>
        Task<Organization> GetOrganization(string userToken);

        /// <summary>
        /// Gets the count of an organization's services from a user token.
        /// </summary>
        Task<int> GetServiceCount(string userToken);
        
        /// <summary>
        /// Gets an organization's service by type from a user token.
        /// </summary>
        Task<Service> GetService<T>(string userToken) where T : ServiceModelBase;

        /// <summary>
        /// Gets all of an organization's services from a user token.
        /// </summary>
        Task<List<Service>> GetServices(string userToken);

        /// <summary>
        /// Gets the latest shapshot for a service from a user token.
        /// </summary>
        /// <param name="createdByUser">Whether or not to get the latest token that was created by the given user</param>
        Task<T> GetLatestServiceData<T>(string userToken, bool createdByUser = false) where T : ServiceModelBase, new();

        /// <summary>
        /// Gets all verified organizations.
        /// </summary>
        Task<List<Organization>> GetVerifiedOrganizations();

        /// <summary>
        /// Gets all users for an organization.
        /// </summary>
        Task<List<User>> GetUsersForOrganization(Organization organization);

        /// <summary>
        /// Clears incomplete snapshots and returns whether or not the conversation was expired.
        /// </summary>
        Task<bool> IsUpdateExpired(string userToken, bool forceExpire);
    }
}
