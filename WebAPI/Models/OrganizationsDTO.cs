using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public class OrganizationDTO
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "Address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "Latitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "Longitude")]
        public string Longitude { get; set; }

#nullable enable
        [JsonProperty("ServiceProvided", NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? ServicesProvided { get; set; }

        [JsonProperty(PropertyName = "Services", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Dictionary<string, Dictionary<string, object>>>? Services { get; set; }
    }
}
