using EntityModel.Luis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace EntityModel
{
    public class SubstanceUseData : ServiceData
    {
        public const string TABLE_NAME = "tira_substanceusedatas";
        public const string PRIMARY_KEY = "_tira_substanceuseserviceid_value";
        public const string SERVICE_NAME = "Substance Use";

        [JsonProperty(PropertyName = "TODO")]
        public int DetoxTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int DetoxOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool DetoxHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool DetoxWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int InPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool InPatientHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool InPatientWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int OutPatientOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool OutPatientHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool OutPatientWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int GroupTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int GroupOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool GroupHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool GroupWaitlistIsOpen { get; set; }

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }
        public override string PrimaryKey() { return PRIMARY_KEY; }
        public override ServiceType ServiceType() { return EntityModel.ServiceType.SubstanceUse; }
        public override string ServiceTypeName() { return SERVICE_NAME; }

        public override List<LuisMapping> LuisMappings()
        {
            return new List<LuisMapping>()
            {
                new LuisMapping()
                {
                    EntityName = nameof(LuisModel.Entities.SubstanceUse),
                    RequestedFlags = ServiceFlags.SubstanceUse
                },
                new LuisMapping()
                {
                    EntityName = nameof(LuisModel.Entities.SubstanceUseDetox),
                    RequestedFlags = ServiceFlags.SubstanceUseDetox
                }
            };
        }

        public override List<SubServiceCategory> ServiceCategories()
        {
            return new List<SubServiceCategory>()
            {
                new SubServiceCategory()
                {
                    Name = SERVICE_NAME,
                    Services = new List<SubService>()
                    {
                        new SubService()
                        {
                            Name = "Substance Use Detox",
                            ServiceFlags = ServiceFlags.SubstanceUse | ServiceFlags.SubstanceUseDetox,

                            TotalPropertyName = nameof(this.DetoxTotal),
                            OpenPropertyName = nameof(this.DetoxOpen),
                            HasWaitlistPropertyName = nameof(this.DetoxHasWaitlist),
                            WaitlistIsOpenPropertyName = nameof(this.DetoxWaitlistIsOpen)
                        },
                        new SubService()
                        {
                            Name = "Substance Use In-Patient",
                            ServiceFlags = ServiceFlags.SubstanceUse,

                            TotalPropertyName = nameof(this.InPatientTotal),
                            OpenPropertyName = nameof(this.InPatientOpen),
                            HasWaitlistPropertyName = nameof(this.InPatientHasWaitlist),
                            WaitlistIsOpenPropertyName = nameof(this.InPatientWaitlistIsOpen)
                        },
                        new SubService()
                        {
                            Name = "Substance Use Out-Patient",
                            ServiceFlags = ServiceFlags.SubstanceUse,

                            TotalPropertyName = nameof(this.OutPatientTotal),
                            OpenPropertyName = nameof(this.OutPatientOpen),
                            HasWaitlistPropertyName = nameof(this.OutPatientHasWaitlist),
                            WaitlistIsOpenPropertyName = nameof(this.OutPatientWaitlistIsOpen)
                        },
                        new SubService()
                        {
                            Name = "Substance Use Group Services",
                            ServiceFlags = ServiceFlags.SubstanceUse,

                            TotalPropertyName = nameof(this.GroupTotal),
                            OpenPropertyName = nameof(this.GroupOpen),
                            HasWaitlistPropertyName = nameof(this.GroupHasWaitlist),
                            WaitlistIsOpenPropertyName = nameof(this.GroupWaitlistIsOpen)
                        }
                    }
                }
            };
        }

        public override void CopyStaticValues<T>(T data)
        {
            var d = data as SubstanceUseData;

            this.DetoxTotal = d.DetoxTotal;
            this.InPatientTotal = d.InPatientTotal;
            this.OutPatientTotal = d.OutPatientTotal;
            this.GroupTotal = d.GroupTotal;

            this.DetoxHasWaitlist = d.DetoxHasWaitlist;
            this.InPatientHasWaitlist = d.InPatientHasWaitlist;
            this.OutPatientHasWaitlist = d.OutPatientHasWaitlist;
            this.GroupHasWaitlist = d.GroupHasWaitlist;

            base.CopyStaticValues(data);
        }

        public class Resolver : ContractResolver<CaseManagementData>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "TODO");
                AddMap(x => x.ServiceId, "TODO");
            }
        }
    }
}
