using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class HousingData : ServiceModelBase
    {
        public static string TABLE_NAME = "tira_housingdatas";
        public static string PRIMARY_KEY = "_tira_housingserviceid_value";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }


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

        public class Resolver : ContractResolver<CaseManagementData>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "tira_housingdataid");
                AddMap(x => x.ServiceId, "_tira_housingserviceid_value");
            }
        }
    }
}
