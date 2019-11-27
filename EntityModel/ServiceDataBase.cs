using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EntityModel
{
    public abstract class ServiceDataBase : ModelBase
    {
        [JsonIgnore]
        public abstract ServiceType ServiceType { get; }

        [JsonIgnore]
        public string ServiceId { get; set; }

        [JsonProperty(PropertyName = "tira_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "createdon")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public string CreatedById { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool IsComplete { get; set; }

        public ServiceDataBase() : base()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.Name = this.CreatedOn.ToString("yyyy/MM/dd hh:mm tt");
        }

        public virtual void CopyStaticValues<T>(T data) where T : ServiceDataBase
        {
            this.ServiceId = data.ServiceId;
        }
    }
}
