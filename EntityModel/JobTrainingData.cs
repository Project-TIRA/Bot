using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public class JobTrainingData : ServiceModelBase
    {
        public static string TABLE_NAME = "TODO";
        public static string PRIMARY_KEY = "TODO";

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
        public string Name { get; set; }

        [JsonProperty(PropertyName = "createdon")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int Total { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int Open { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int WaitlistLength { get; set; }

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
