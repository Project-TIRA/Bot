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

        [JsonProperty(PropertyName = "tira_longtemsharedbedstotal")]
        public int LongTermSharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedsopen")]
        public int LongTermSharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedswaitlist")]
        public int LongTermSharedBedsWaitListLength{ get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedstotal")]
        public int LongTermPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedsopen")]
        public int LongTermPrivateBedsOpen { get; set; }        

        [JsonProperty(PropertyName = "tira_longtemprivatebedswaitlist")]
        public int LongTermPrivateBedsWaitListLength { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedstotal")]
        public int EmergencySharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedsopen")]
        public int EmergencySharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedswaitlist")]
        public int EmergencySharedBedsWaitListLength { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedstotal")]
        public int EmergencyPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedsopen")]
        public int EmergencyPrivatedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedswaitlist")]
        public int EmergencyPrivateBedsWaitListLength { get; set; }

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
