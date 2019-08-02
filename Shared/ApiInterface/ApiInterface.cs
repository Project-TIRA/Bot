using EntityModel;
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
        /// Gets the latest shapshot for a service from a user token.
        /// </summary>
        Task<T> GetLatestServiceData<T>(string userToken) where T : ServiceModelBase;
    }

    public enum ServiceType
    {
        Invalid = 0,
        Housing = 1,
        CaseManagement = 2,
        MentalHealth = 3,
        SubstanceUse = 4,
        JobTraining = 5
    }
}
