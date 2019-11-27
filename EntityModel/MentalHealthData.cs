using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class MentalHealthData : ServiceDataBase
    {
        public static string TABLE_NAME = "tira_substanceuses";
        public static string PRIMARY_KEY = "TODO";

        [JsonIgnore]
        public override ServiceType ServiceType { get { return ServiceType.MentalHealth; } }

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }


        [JsonProperty(PropertyName = "TODO")]
        public int InPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool InPatientHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool InPatientWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool OutPatientHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool OutPatientWaitlistIsOpen { get; set; }

        public override void CopyStaticValues<T>(T data)
        {
            var d = data as MentalHealthData;

            this.InPatientTotal = d.InPatientTotal;
            this.OutPatientTotal = d.OutPatientTotal;

            this.InPatientHasWaitlist = d.InPatientHasWaitlist;
            this.OutPatientHasWaitlist = d.OutPatientHasWaitlist;

            base.CopyStaticValues(data);
        }

        public class Resolver : ContractResolver<CaseManagementData>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "TODO");
                AddMap(x => x.ServiceId, "TODO");
            }
        }
    }
}
