using EntityModel.Helpers;
using Newtonsoft.Json;

namespace WebAPI.Models
{
    public class ServicesDTO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_servicetype")]
        public ServiceType Type { get; set; }
    }
}
