using Newtonsoft.Json;

namespace Shared.Models
{
    public class CaseManagementData : ModelBase
    {
        public static string TABLE_NAME = "tira_casemanagementdatas";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override string ResourceId { get { return Id; } }

        [JsonProperty(PropertyName = "tira_casemanagementdataid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_tira_casemanagementserviceid_value")]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_haswaitlist")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_waitlistlength")]
        public int WaitlistLength { get; set; }

        [JsonProperty(PropertyName = "tira_spotstotal")]
        public int SpotsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_spotsopen")]
        public int SpotsOpen { get; set; }

        // Called by Json to prevent serialization but allow deserialization.
        public bool ShouldSerializeId()
        {
            return false;
        }

        // Called by Json to prevent serialization but allow deserialization.
        public bool ShouldSerializeServiceId()
        {
            return false;
        }

    }
}
