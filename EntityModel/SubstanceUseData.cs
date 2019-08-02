using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public class SubstanceUseData : ServiceModelBase
    {
        public static string TABLE_NAME = "tira_substanceusedatas";
        public static string PRIMARY_KEY = "_tira_substanceuseserviceid_value";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override string ResourceId { get { return Id; } }

        [Key]
        [JsonProperty(PropertyName = "TODO")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int DetoxWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int GroupWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int DetoxTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int GroupTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int DetoxOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
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
