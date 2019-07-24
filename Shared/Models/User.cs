using Newtonsoft.Json;

namespace Shared.Models
{
    public class User
    {
        public static string TableName = "contacts";

        [JsonProperty(PropertyName = "contactId")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_accountid_value")]
        public string OrganizationId { get; set; }
    }
}
