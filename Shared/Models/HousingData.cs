using Newtonsoft.Json;

namespace Shared.Models
{
    public class HousingData : ModelBase
    {
        public static string TABLE_NAME = "tira_housingdatas";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override string ResourceId { get { return Id; } }

        [JsonProperty(PropertyName = "tira_housingdataid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_tira_housingserviceid_value")]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_haswaitlist")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedswaitlist", NullValueHandling = NullValueHandling.Ignore)]
        public int LongTermSharedBedsWaitListLength{ get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedswaitlist", NullValueHandling = NullValueHandling.Ignore)]
        public int LongTermPrivateBedsWaitListLength { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedswaitlist", NullValueHandling = NullValueHandling.Ignore)]
        public int EmergencySharedBedsWaitListLength { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedswaitlist", NullValueHandling = NullValueHandling.Ignore)]
        public int EmergencyPrivateBedsWaitListLength { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedsopen", NullValueHandling = NullValueHandling.Ignore)]
        public int LongTermSharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedstotal")]
        public int LongTermSharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedsopen", NullValueHandling = NullValueHandling.Ignore)]
        public int LongTermPrivateBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedstotal")]
        public int LongTermPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedsopen", NullValueHandling = NullValueHandling.Ignore)]
        public int EmergencySharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedstotal")]
        public int EmergencySharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedsopen", NullValueHandling = NullValueHandling.Ignore)]
        public int EmergencyPrivatedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedstotal")]
        public int EmergencyPrivateBedsTotal { get; set; }

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
