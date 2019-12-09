using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

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

    [Flags]
    public enum ServiceFlags : int
    {
        None = 0,
        CaseManagement = 1 << 0,
        Employment = 1 << 1,
        EmploymentInternship = 1 << 2,
        HousingEmergency = 1 << 3,
        HousingLongTerm = 1 << 4,
        MentalHealth = 1 << 5,
        SubstanceUse = 1 << 6,
        SubstanceUseDetox = 1 << 7,
    }
}
