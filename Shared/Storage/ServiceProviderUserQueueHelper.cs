using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Shared.Storage
{
    public class ServiceProviderUserQueueHelper : QueueHelper
    {
        public const string QueueName = "service-provider-users";

        private string connectionString;

        public ServiceProviderUserQueueHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task Enqueue(ServiceProviderUserQueueData data)
        {
            await AddMessage(this.connectionString, QueueName, JsonConvert.SerializeObject(data));
        }

        public async Task<ServiceProviderUserQueueData> GetMessage()
        {
            var message = await GetMessage(this.connectionString, QueueName);
            return JsonConvert.DeserializeObject<ServiceProviderUserQueueData>(message);
        }

        public async Task DeleteMessage(ServiceProviderUserQueueData message)
        {
            await DeleteMessage(this.connectionString, QueueName, JsonConvert.SerializeObject(message));
        }
    }

    public class ServiceProviderUserQueueData
    {
        public string UserId { get; set; }
        public string LatestUpdateString { get; set; }
    }
}
