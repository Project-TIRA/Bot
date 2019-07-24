using Newtonsoft.Json;

namespace Shared.Models
{
    public class User : ModelBase
    {
        public static string TABLE_NAME = "contacts";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override string ResourceId { get { return Id; } }

        [JsonProperty(PropertyName = "contactId")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_parentcustomerid_value")]
        public string OrganizationId { get; set; }

        // Called by Json to prevent serialization but allow deserialization.
        public bool ShouldSerializeId()
        {
            return false;
        }

        // Called by Json to prevent serialization but allow deserialization.
        public bool ShouldSerializeOrganizationId()
        {
            return false;
        }
    }
}
