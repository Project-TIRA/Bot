using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EntityModel
{
    public class CaseManagementData : ServiceModelBase
    {
        public static string TABLE_NAME = "TODO";
        public static string PRIMARY_KEY = "TODO";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }
        

        [JsonProperty(PropertyName = "TODO")]
        public int Total { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int Open { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int WaitlistLength { get; set; }

        public override void CopyStaticValues<T>(T data)
        {
            var d = data as CaseManagementData;

            this.Total = d.Total;
            this.HasWaitlist = d.HasWaitlist;

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
