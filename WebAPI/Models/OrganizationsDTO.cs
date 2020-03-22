using System;
using System.Collections.Generic;

using System.Globalization;
using EntityModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebAPI.Models
{
    public class OrganizationDTO
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "Address")]
        public string Address { get; set; }

#nullable enable

        [JsonProperty("ServiceProvided", NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? ServicesProvided { get; set; }

        [JsonProperty(PropertyName = "Services", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ServiceCategoryDTO>? Services { get; set; }

#nullable disable



    }
    public class ServiceCategoryDTO
    {
        [JsonIgnore]
        public string Name { get; set; }
        public Dictionary<string, ServiceDataDTO> ServicesData { get; set; }
        public ServiceCategoryDTO()
        {
            this.ServicesData = new Dictionary<string, ServiceDataDTO>();
        }
    }

    public class ServiceDataDTO
    {
        [JsonIgnore]
        public string Name { get; set; }
        public Object TotalPropertyName { get; set; }
        public Object OpenPropertyName { get; set; }
        public Object HasWaitlistPropertyName { get; set; }
        public Object WaitlistIsOpenPropertyName { get; set; }
    }

}