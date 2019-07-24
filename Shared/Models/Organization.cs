using Newtonsoft.Json;

namespace Shared.Models
{
    public class Organization
    {
        public static string TableName = "accounts";

        [JsonProperty(PropertyName = "accountid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_isverified")]
        public string IsVerified { get; set; }

        [JsonProperty(PropertyName = "tira_updatefrequency")]
        public string UpdateFrequency { get; set; }
    }
}
