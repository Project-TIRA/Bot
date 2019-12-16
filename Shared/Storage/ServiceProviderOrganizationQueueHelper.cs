using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Storage
{
    public class ServiceProviderOrganizationQueueHelper : QueueHelper
    {
        public const string QueueName = "service-provider-organizations";

        private string connectionString;

        public ServiceProviderOrganizationQueueHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task AddMessage(ServiceProviderOrganizationQueueData message)
        {
            await AddMessage(this.connectionString, QueueName, JsonConvert.SerializeObject(message));
        }

        public async Task<(CloudQueueMessage Message, ServiceProviderOrganizationQueueData data)> GetMessage()
        {
            var message = await GetMessage(this.connectionString, QueueName);
            var data = JsonConvert.DeserializeObject<ServiceProviderOrganizationQueueData>(message.AsString);
            return (message, data);
        }

        public async Task DeleteMessage(CloudQueueMessage message)
        {
            await DeleteMessage(this.connectionString, QueueName, message);
        }
    }

    public class ServiceProviderOrganizationQueueData
    {
        public string OrganizationId { get; set; }
        public List<string> UserIds { get; set; }

        public ServiceProviderOrganizationQueueData()
        {
            this.UserIds = new List<string>();
        }
    }
}
