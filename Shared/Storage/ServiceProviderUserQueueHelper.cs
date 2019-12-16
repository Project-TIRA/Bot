using Microsoft.Azure.Storage.Queue;
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

        public async Task<(CloudQueueMessage Message, ServiceProviderUserQueueData data)> GetMessage()
        {
            var message = await GetMessage(this.connectionString, QueueName);
            var data = JsonConvert.DeserializeObject<ServiceProviderUserQueueData>(message.AsString);
            return (message, data);
        }

        public async Task DeleteMessage(CloudQueueMessage message)
        {
            await DeleteMessage(this.connectionString, QueueName, message);
        }
    }

    public class ServiceProviderUserQueueData
    {
        public string UserId { get; set; }
        public string LatestUpdateString { get; set; }
    }
}
