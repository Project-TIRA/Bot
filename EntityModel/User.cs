using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EntityModel
{
    public class User : Model
    {
        public static string TABLE_NAME = "contacts";

        [JsonIgnore]
        [JsonProperty(PropertyName = "_parentcustomerid_value")]
        public string OrganizationId { get; set; }

        [JsonProperty(PropertyName = "firstname")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "mobilephone")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "tira_updatefrequency")]
        public Day ReminderFrequency { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool ContactEnabled { get; set; }

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }

        public class Resolver : ContractResolver<CaseManagementData>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "contactId");
            }
        }
    }

    [Flags]
    public enum Day
    {
        None = 0,
        Sunday = (1 << 0),
        Monday = (1 << 1),
        Tuesday = (1 << 2),
        Wednesday = (1 << 3),
        Thursday = (1 << 4),
        Friday = (1 << 5),
        Saturday = (1 << 6)
    }
}
