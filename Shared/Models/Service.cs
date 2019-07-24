using Newtonsoft.Json;

namespace Shared.Models
{
    public class Service
    {
        public static string TableName = "tira_services";

        [JsonProperty(PropertyName = "tira_serviceid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_tira_organizationservicesid_value")]
        public string OrganizationId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_servicetype")]
        public string ServiceType { get; set; }
    }
}
