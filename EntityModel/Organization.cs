using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class Organization : Model
    {
        public static string TABLE_NAME = "accounts";

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

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }

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
