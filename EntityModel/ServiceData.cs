using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityModel
{
    public abstract class ServiceData : Model
    {
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

        public abstract string PrimaryKey();
        public abstract ServiceType ServiceType();
        public abstract string ServiceTypeName();
        public abstract List<string> LuisEntityNames();

        public abstract List<SubServiceCategory> ServiceCategories();

        public ServiceData() : base()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.Name = this.CreatedOn.ToString("yyyy/MM/dd hh:mm tt");
        }

        public object GetProperty(string property)
        {
            return GetType().GetProperty(property).GetValue(this);
        }

        public void SetProperty(string property, object value)
        {
            GetType().GetProperty(property).SetValue(this, value);
        }

        public virtual void CopyStaticValues<T>(T data) where T : ServiceData
        {
            this.ServiceId = data.ServiceId;
        }
    }

    public class SubServiceCategory
    {
        public string Name { get; set; }
        public List<SubService> Services { get; set; }

        public SubServiceCategory()
        {
            this.Services = new List<SubService>();
        }

        public ServiceFlags ServiceFlags()
        {
            ServiceFlags flags = EntityModel.ServiceFlags.None;
            this.Services.ForEach(s => flags |= s.ServiceFlags);
            return flags;
        }
    }

    public class SubService
    {
        public string Name { get; set; }
        public ServiceFlags ServiceFlags { get; set; }
        public List<string> LuisEntityNames { get; set; }

        public string TotalPropertyName { get; set; }
        public string OpenPropertyName { get; set; }
        public string HasWaitlistPropertyName { get; set; }
        public string WaitlistIsOpenPropertyName { get; set; }

        public SubService()
        {
            this.LuisEntityNames = new List<string>();
        }
    }
}
