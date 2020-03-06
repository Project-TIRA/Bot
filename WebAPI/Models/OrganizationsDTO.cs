using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebAPI.Models
{
    public class OrganizationDTO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "address1_composite")]
        public string Address { get; set; }



    }



}