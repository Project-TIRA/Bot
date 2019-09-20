using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EntityModel
{
    public class Feedback : ModelBase
    {
        public static string TABLE_NAME = "TODO";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }


        [JsonIgnore]
        [JsonProperty(PropertyName = "TODO")]
        public string SenderId { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public string Text { get; set; }

        public Feedback() : base()
        {
            this.CreatedOn = DateTime.UtcNow;
        }

        public class Resolver : ContractResolver<Feedback>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "TODO");
            }
        }
    }
}
