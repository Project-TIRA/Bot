using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class Organization : ModelBase
    {
        public static string TABLE_NAME = "accounts";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }
   
        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }


        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_isverified")]
        public bool IsVerified { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "address1_composite")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "to_do")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "to_do")]
        public string Longitude { get; set; }

        public class Resolver : ContractResolver<CaseManagementData>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "accountid");
            }
        }
    }
}
