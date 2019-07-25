using Newtonsoft.Json;

namespace Shared.Models
{
    public class SubstanceUseData : ModelBase
    {
        public static string TABLE_NAME = "tira_substanceuses";
        public static string PRIMARY_KEY = "TODO";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override string ResourceId { get { return Id; } }

        [JsonProperty(PropertyName = "TODO")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int DetoxTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int DetoxOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int DetoxWaitListLength { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientWaitListLength { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientWaitListLength { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int GroupTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int GroupOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int GroupWaitListLength { get; set; }

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
