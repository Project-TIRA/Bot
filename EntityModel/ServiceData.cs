using EntityModel.Helpers;
using EntityModel.Luis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// The primary key for referencing the type in the data store.
        /// </summary>
        public abstract string PrimaryKey();

        /// <summary>
        /// The service type.
        /// </summary>
        public abstract ServiceType ServiceType();

        /// <summary>
        /// The friendly name of the service type.
        /// </summary>
        public abstract string ServiceTypeName();

        /// <summary>
        /// The LUIS entity mappings that are handled by the type.
        /// </summary>
        public abstract List<LuisMapping> LuisMappings();

        /// <summary>
        /// The sub-services of the type, grouped by category.
        /// The categories are used to clarify searches for services.
        /// </summary>
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

        public override string ToString()
        {
            var result = string.Empty;
            
            foreach (var serviceCategory in this.ServiceCategories())
            {
                foreach (var subService in serviceCategory.Services)
                {
                    var total = (int)GetProperty(subService.TotalPropertyName);
                    if (total == 0)
                    {
                        continue;
                    }

                    // Add a newline if there is already some text.
                    result += string.IsNullOrEmpty(result) ? string.Empty : "\n";

                    var open = (int)GetProperty(subService.OpenPropertyName);
                    if (open == 0)
                    {
                        // If there are no openings but there is a waitlist, give the waitlist status.
                        var hasWaitlist = (bool)GetProperty(subService.HasWaitlistPropertyName);
                        if (hasWaitlist)
                        {
                            var waitlistIsOpen = (bool)GetProperty(subService.WaitlistIsOpenPropertyName);
                            result += $"- {subService.Name}: waitlist {(waitlistIsOpen ? "open" : "closed")}";
                            continue;
                        }

                    }

                    // If there are openings, or there are no openings but no waitlist, give the number of openings.
                    result += $"- {subService.Name}: {GetProperty(subService.OpenPropertyName)}";
                }
            }

            return result;
        }
    }

    public class SubServiceCategory
    {
        public string Name { get; set; }
        public List<SubService> Services { get; set; }
        public ServiceFlags ServiceFlags { get { return this.Services.Select(s => s.ServiceFlags).Aggregate(ServiceFlags.None, (f1, f2) => f1 |= f2); } }

        public SubServiceCategory()
        {
            this.Services = new List<SubService>();
        }
    }

    public class SubService
    {
        public string Name { get; set; }
        public ServiceFlags ServiceFlags { get; set; }

        public string TotalPropertyName { get; set; }
        public string OpenPropertyName { get; set; }
        public string HasWaitlistPropertyName { get; set; }
        public string WaitlistIsOpenPropertyName { get; set; }
    }
}
