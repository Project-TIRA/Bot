using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public class User : ModelBase
    {
        public static string TABLE_NAME = "contacts";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override string ResourceId { get { return Id; } }

        [Key]
        [JsonProperty(PropertyName = "contactId")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_parentcustomerid_value")]
        public string OrganizationId { get; set; }

        [JsonProperty(PropertyName = "firstname")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "mobilephone")]
        public string PhoneNumber { get; set; }

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
