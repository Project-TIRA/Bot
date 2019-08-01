using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public class HousingData : ServiceModelBase
    {
        public static string TABLE_NAME = "tira_housingdatas";
        public static string PRIMARY_KEY = "_tira_housingserviceid_value";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override string ResourceId { get { return Id; } }

        [Key]
        [JsonProperty(PropertyName = "tira_housingdataid")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_tira_housingserviceid_value")]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "createdon")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "tira_haswaitlist")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedstotal")]
        public int LongTermSharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedsopen")]
        public int LongTermSharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedswaitlist")]
        public int LongTermSharedBedsWaitlistLength{ get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedstotal")]
        public int LongTermPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedsopen")]
        public int LongTermPrivateBedsOpen { get; set; }        

        [JsonProperty(PropertyName = "tira_longtemprivatebedswaitlist")]
        public int LongTermPrivateBedsWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedstotal")]
        public int EmergencySharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedsopen")]
        public int EmergencySharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedswaitlist")]
        public int EmergencySharedBedsWaitlistLength { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedstotal")]
        public int EmergencyPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedsopen")]
        public int EmergencyPrivateBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedswaitlist")]
        public int EmergencyPrivateBedsWaitlistLength { get; set; }

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
