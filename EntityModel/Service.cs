using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public class Service : ModelBase
    {
        public static string TABLE_NAME = "tira_services";

        [JsonIgnore]
        public override string TableName {  get { return TABLE_NAME; } }

        public override string ResourceId { get { return Id; } }

        [Key]
        [JsonProperty(PropertyName = "tira_serviceid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_tira_organizationservicesid_value")]
        public string OrganizationId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_servicetype")]
        public int Type { get; set; }

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
