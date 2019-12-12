using EntityModel.Luis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace EntityModel
{
    public class HousingData : ServiceData
    {
        public const string TABLE_NAME = "tira_housingdatas";
        public const string PRIMARY_KEY = "_tira_housingserviceid_value";
        public const string SERVICE_NAME = "Housing";

        [JsonProperty(PropertyName = "tira_emergencysharedbedstotal")]
        public int EmergencySharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedsopen")]
        public int EmergencySharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool EmergencySharedBedsHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_emergencysharedbedswaitlist")]
        public bool EmergencySharedBedsWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedstotal")]
        public int EmergencyPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedsopen")]
        public int EmergencyPrivateBedsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool EmergencyPrivateBedsHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_emergencyprivatebedswaitlist")]
        public bool EmergencyPrivateBedsWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedstotal")]
        public int LongTermSharedBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedsopen")]
        public int LongTermSharedBedsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool LongTermSharedBedsHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_longtemsharedbedswaitlist")]
        public bool LongTermSharedBedsWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedstotal")]
        public int LongTermPrivateBedsTotal { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedsopen")]
        public int LongTermPrivateBedsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool LongTermPrivateBedsHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "tira_longtemprivatebedswaitlist")]
        public bool LongTermPrivateBedsWaitlistIsOpen { get; set; }

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }
        public override string PrimaryKey() { return PRIMARY_KEY; }
        public override ServiceType ServiceType() { return EntityModel.ServiceType.Housing; }
        public override string ServiceTypeName() { return SERVICE_NAME; }

        public override List<LuisMapping> LuisMappings()
        {
            return new List<LuisMapping>()
            {
                new LuisMapping()
                {
                    EntityName = nameof(LuisModel.Entities.Housing),
                    ServiceFlags = ServiceFlags.None
                },
                new LuisMapping()
                {
                    EntityName = nameof(LuisModel.Entities.HousingEmergency),
                    ServiceFlags = ServiceFlags.HousingEmergency
                },
                new LuisMapping()
                {
                    EntityName = nameof(LuisModel.Entities.HousingLongTerm),
                    ServiceFlags = ServiceFlags.HousingLongTerm
                }
            };
        }

        public override List<SubServiceCategory> ServiceCategories()
        {
            return new List<SubServiceCategory>()
            {
                new SubServiceCategory()
                {
                    Name = "Emergency",
                    Services = new List<SubService>()
                    {
                        new SubService()
                        {
                            Name = "Emergency Shared-Space Beds",
                            ServiceFlags = ServiceFlags.HousingEmergency,

                            TotalPropertyName = nameof(this.EmergencySharedBedsTotal),
                            OpenPropertyName = nameof(this.EmergencySharedBedsOpen),
                            HasWaitlistPropertyName = nameof(this.EmergencySharedBedsHasWaitlist),
                            WaitlistIsOpenPropertyName = nameof(this.EmergencySharedBedsWaitlistIsOpen)
                        },
                        new SubService()
                        {
                            Name = "Emergency Private Beds",
                            ServiceFlags = ServiceFlags.HousingEmergency,

                            TotalPropertyName = nameof(this.EmergencyPrivateBedsTotal),
                            OpenPropertyName = nameof(this.EmergencyPrivateBedsOpen),
                            HasWaitlistPropertyName = nameof(this.EmergencyPrivateBedsHasWaitlist),
                            WaitlistIsOpenPropertyName = nameof(this.EmergencyPrivateBedsWaitlistIsOpen)
                        }
                    }
                },
                new SubServiceCategory()
                {
                    Name = "Long-Term",
                    Services = new List<SubService>()
                    {
                        new SubService()
                        {
                            Name = "Long-Term Shared-Space Beds",
                            ServiceFlags = ServiceFlags.HousingLongTerm,

                            TotalPropertyName = nameof(this.LongTermSharedBedsTotal),
                            OpenPropertyName = nameof(this.LongTermSharedBedsOpen),
                            HasWaitlistPropertyName = nameof(this.LongTermSharedBedsHasWaitlist),
                            WaitlistIsOpenPropertyName = nameof(this.LongTermSharedBedsWaitlistIsOpen)
                        },
                        new SubService()
                        {
                            Name = "Long-Term Private Beds",
                            ServiceFlags = ServiceFlags.HousingLongTerm,

                            TotalPropertyName = nameof(this.LongTermPrivateBedsTotal),
                            OpenPropertyName = nameof(this.LongTermPrivateBedsOpen),
                            HasWaitlistPropertyName = nameof(this.LongTermPrivateBedsHasWaitlist),
                            WaitlistIsOpenPropertyName = nameof(this.LongTermPrivateBedsWaitlistIsOpen)
                        }
                    }
                }
            };
        }

        public override void CopyStaticValues<T>(T data)
        {
            var d = data as HousingData;

            this.EmergencySharedBedsTotal = d.EmergencySharedBedsTotal;
            this.EmergencyPrivateBedsTotal = d.EmergencyPrivateBedsTotal;
            this.LongTermSharedBedsTotal = d.LongTermSharedBedsTotal;
            this.LongTermPrivateBedsTotal = d.LongTermPrivateBedsTotal;

            this.EmergencySharedBedsHasWaitlist = d.EmergencySharedBedsHasWaitlist;
            this.EmergencyPrivateBedsHasWaitlist = d.EmergencyPrivateBedsHasWaitlist;
            this.LongTermSharedBedsHasWaitlist = d.LongTermSharedBedsHasWaitlist;
            this.LongTermPrivateBedsHasWaitlist = d.LongTermPrivateBedsHasWaitlist;

            base.CopyStaticValues(data);
        }

        public class Resolver : ContractResolver<CaseManagementData>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "tira_housingdataid");
                AddMap(x => x.ServiceId, "_tira_housingserviceid_value");
            }
        }
    }
}
