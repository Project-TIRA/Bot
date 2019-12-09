using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

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

        public abstract List<SubService> SubServices();

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

        public virtual List<SubServiceCategory> SubServiceCategories()
        {
            return new List<SubServiceCategory>();
        }

        public virtual void CopyStaticValues<T>(T data) where T : ServiceData
        {
            this.ServiceId = data.ServiceId;
        }
    }

    public class SubService
    {
        public string Name { get; set; }
        public ServiceFlags ServiceFlag { get; set; }
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

    public class SubServiceCategory
    {
        public string Name { get; set; }
        public ServiceFlags ServiceFlag { get; set; }
    }
}
