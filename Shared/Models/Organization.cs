using Newtonsoft.Json;

namespace Shared.Models
{
    public class Organization : ModelBase
    {
        public static string TABLE_NAME = "accounts";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override string ResourceId { get { return Id; } }

        [JsonProperty(PropertyName = "accountid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_isverified")]
        public bool IsVerified { get; set; }

        [JsonProperty(PropertyName = "tira_updatefrequency")]
        public int UpdateFrequency { get; set; }

        // Called by Json to prevent serialization but allow deserialization.
        public bool ShouldSerializeId()
        {
            return false;
        }
    }
}
