using EntityModel;
using System.Threading.Tasks;

namespace Shared.ApiInterface
{
    public abstract class ApiInterface
    {
        /// <summary>
        /// Creates a new record of a model.
        /// </summary>
        public abstract Task<string> Create<T>(T model) where T : ModelBase;

        /// <summary>
        /// Saves changes to a model.
        /// </summary>
        public abstract Task<bool> Update<T>(T model) where T : ModelBase;

        /// <summary>
        /// Gets a user from a user ID.
        /// </summary>
        public abstract Task<User> GetUser(string userId);

        /// <summary>
        /// Gets an organization from a user ID.
        /// </summary>
        public abstract Task<Organization> GetOrganization(string userId);

        /// <summary>
        /// Gets the count of an organization's services from a user ID.
        /// </summary>
        public abstract Task<int> GetServiceCount(string userId);

        /// <summary>
        /// Gets an organization's service by type from a user ID.
        /// </summary>
        public abstract Task<Service> GetService<T>(string userId) where T : ServiceModelBase;

        /// <summary>
        /// Gets the latest shapshot for a service from a user ID.
        /// </summary>
        public abstract Task<T> GetLatestServiceData<T>(string userId) where T : ServiceModelBase;
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
