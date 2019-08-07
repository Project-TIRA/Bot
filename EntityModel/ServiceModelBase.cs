using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EntityModel
{
    public abstract class ServiceModelBase : ModelBase
    {
        [JsonIgnore]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "createdon")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public string CreatedById { get; set; }

        [JsonProperty(PropertyName = "tira_haswaitlist")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool IsComplete { get; set; }

        public ServiceModelBase() : base()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.Name = this.CreatedOn.ToString("yyyy/MM/dd hh:mm tt K");
        }

        public virtual void CopyStaticValues<T>(T data) where T : ServiceModelBase
        {
            this.ServiceId = data.ServiceId;
            this.HasWaitlist = data.HasWaitlist;
        }
    }
}
