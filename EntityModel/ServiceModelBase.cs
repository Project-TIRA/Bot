using Newtonsoft.Json;
using System;

namespace EntityModel
{
    public abstract class ServiceModelBase : ModelBase
    {
        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "createdon")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "tira_haswaitlist")]
        public bool HasWaitlist { get; set; }
    }
}
