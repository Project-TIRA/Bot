using EntityModel.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public class ServicesDTO
    {
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonIgnoreAttribute]
        [JsonProperty(PropertyName = "Type")]
        public ServiceType Type { get; set; }

#nullable enable

        [JsonProperty(PropertyName = "Categories")]
        public Dictionary<string, HashSet<string>>? ServicesCategories { get; set; }

#nullable disable
    }
}