using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class HousingData : ServiceDataBase
    {
        public static string TABLE_NAME = "tira_housingdatas";
        public static string PRIMARY_KEY = "_tira_housingserviceid_value";

        [JsonIgnore]
        public override ServiceType ServiceType { get { return ServiceType.Housing; } }

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }


        [JsonProperty(PropertyName = "tira_emergencysharedbedstotal")]
        public int EmergencySharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedsopen")]
        public int EmergencySharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool EmergencySharedBedsHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedswaitlist")]
        public bool EmergencySharedBedsWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedstotal")]
        public int EmergencyPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedsopen")]
        public int EmergencyPrivateBedsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool EmergencyPrivateBedsHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedswaitlist")]
        public bool EmergencyPrivateBedsWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedstotal")]
        public int LongTermSharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedsopen")]
        public int LongTermSharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool LongTermSharedBedsHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedswaitlist")]
        public bool LongTermSharedBedsWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedstotal")]
        public int LongTermPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedsopen")]
        public int LongTermPrivateBedsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool LongTermPrivateBedsHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedswaitlist")]
        public bool LongTermPrivateBedsWaitlistIsOpen { get; set; }

        public override void CopyStaticValues<T>(T data)
        {
            var d = data as HousingData;

            this.EmergencySharedBedsTotal = d.EmergencySharedBedsTotal;
            this.EmergencyPrivateBedsTotal = d.EmergencyPrivateBedsTotal;
            this.LongTermSharedBedsTotal = d.LongTermSharedBedsTotal;
            this.LongTermPrivateBedsTotal = d.LongTermPrivateBedsTotal;

            this.EmergencySharedBedsHasWaitlist = d.EmergencySharedBedsHasWaitlist;
            this.EmergencyPrivateBedsHasWaitlist = d.EmergencyPrivateBedsHasWaitlist;
            this.LongTermSharedBedsHasWaitlist = d.LongTermSharedBedsHasWaitlist;
            this.LongTermPrivateBedsHasWaitlist = d.LongTermPrivateBedsHasWaitlist;

            base.CopyStaticValues(data);
        }

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
