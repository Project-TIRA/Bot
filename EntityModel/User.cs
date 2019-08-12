using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class User : ModelBase
    {
        public static string TABLE_NAME = "contacts";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }


        [JsonIgnore]
        [JsonProperty(PropertyName = "_parentcustomerid_value")]
        public string OrganizationId { get; set; }

        [JsonProperty(PropertyName = "firstname")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "mobilephone")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool ContactEnabled { get; set; }

        public class Resolver : ContractResolver<CaseManagementData>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "contactId");
            }
        }
    }
}
