using System;
using System.Collections.Generic;

using System.Globalization;
using EntityModel.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebAPI.Models
{
    public class ServiceDTO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tira_servicetype")]
        public ServiceType Type { get; set; }



    }



}