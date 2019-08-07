using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class SubstanceUseData : ServiceModelBase
    {
        public static string TABLE_NAME = "tira_substanceusedatas";
        public static string PRIMARY_KEY = "_tira_substanceuseserviceid_value";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }


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

        public override void CopyStaticValues<T>(T data)
        {
            var d = data as SubstanceUseData;
            this.DetoxTotal = d.DetoxTotal;
            this.GroupTotal = d.GroupTotal;
            this.InPatientTotal = d.InPatientTotal;
            this.OutPatientTotal = d.OutPatientTotal;

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
