using Newtonsoft.Json;

namespace Shared.Models
{
    public class SubstanceUseData : ModelBase
    {
        public static string TABLE_NAME = "tira_substanceusedatas";
        public static string PRIMARY_KEY = "_tira_substanceuseserviceid_value";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override string ResourceId { get { return Id; } }

        [JsonProperty(PropertyName = "tira_substanceusedataid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_tira_substanceuseserviceid_value")]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_haswaitlist")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_detoxwaitlistlength")]
        public int DetoxWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "tira_inpatientwaitlistlength")]
        public int InPatientWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "tira_outpatientwaitlistlength")]
        public int OutPatientWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "tira_groupwaitlistlength")]
        public int GroupWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "tira_detoxtotal")]
        public int DetoxTotal { get; set; }

        [JsonProperty(PropertyName = "tira_inpatienttotal")]
        public int InPatientTotal { get; set; }

        [JsonProperty(PropertyName = "tira_outpatienttotal")]
        public int OutPatientTotal { get; set; }

        [JsonProperty(PropertyName = "tira_grouptotal")]
        public int GroupTotal { get; set; }

        [JsonProperty(PropertyName = "tira_detoxopen")]
        public int DetoxOpen { get; set; }

        [JsonProperty(PropertyName = "tira_inpatientopen")]
        public int InPatientOpen { get; set; }

        [JsonProperty(PropertyName = "tira_outpatientopen")]
        public int OutPatientOpen { get; set; }

        [JsonProperty(PropertyName = "tira_groupopen")]
        public int GroupOpen { get; set; }

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
