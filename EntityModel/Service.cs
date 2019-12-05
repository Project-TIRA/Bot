using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class Service : Model
    {
        public static string TABLE_NAME = "tira_services";

        [JsonIgnore]
        [JsonProperty(PropertyName = "_tira_organizationservicesid_value")]
        public string OrganizationId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_servicetype")]
        public ServiceType Type { get; set; }

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }

        public class Resolver : ContractResolver<CaseManagementData>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "tira_serviceid");
            }
        }
    }

    public enum ServiceType : int
    {
        Invalid = 0,
        Housing = 1,
        CaseManagement = 2,
        MentalHealth = 3,
        SubstanceUse = 4,
        Employment = 5
    }
}
