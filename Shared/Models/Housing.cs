using Newtonsoft.Json;

namespace Shared.Models
{
    public class Housing
    {
        public static string TableName = "tira_housingdatas";

        [JsonProperty(PropertyName = "_tira_housingserviceid_value")]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_haswaitlist")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedswaitlist")]
        public int LongTermSharedBedsWaitList{ get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedswaitlist")]
        public int LongTermPrivateBedsWaitList { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedswaitlist")]
        public int EmergencySharedBedsWaitList { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedswaitlist")]
        public int EmergencyPrivateBedsWaitList { get; set; }

        [JsonProperty(PropertyName = "tira_longtermsharedbedsopen")]
        public int LongTermSharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedstotal")]
        public int LongTermSharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedsopen")]
        public int LongTemPrivateBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedstotal")]
        public int LongTemPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedsopen")]
        public int EmergencySharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedstotal")]
        public int EmergencySharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedsopen")]
        public int EmergencyPrivatedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedstotal")]
        public int EmergencyPrivateBedsTotal { get; set; }

    }
}
