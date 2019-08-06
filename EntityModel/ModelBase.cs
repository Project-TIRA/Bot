using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public abstract class ModelBase
    {
        [Key]
        [JsonIgnore]
        public string Id { get; set; }

        [JsonIgnore]
        public abstract string TableName { get; }

        [JsonIgnore]
        public abstract IContractResolver ContractResolver { get; }

        public ModelBase()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
