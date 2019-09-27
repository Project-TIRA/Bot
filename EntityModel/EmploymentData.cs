using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class EmploymentData : ServiceDataBase
    {
        public static string TABLE_NAME = "TODO";
        public static string PRIMARY_KEY = "TODO";

        [JsonIgnore]
        public override string TableName { get { return TABLE_NAME; } }

        [JsonIgnore]
        public override IContractResolver ContractResolver { get { return Resolver.Instance; } }

        [JsonProperty(PropertyName = "TODO")]
        public int JobReadinessTrainingTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int JobReadinessTrainingOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool JobReadinessTrainingHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool JobReadinessTrainingWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int PaidInternshipTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int PaidInternshipOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool PaidInternshipHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool PaidInternshipWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int VocationalTrainingTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int VocationalTrainingOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool VocationalTrainingHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool VocationalTrainingWaitlistIsOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int EmploymentPlacementTotal { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public int EmploymentPlacementOpen { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool EmploymentPlacementHasWaitlist { get; set; }

        [JsonProperty(PropertyName = "TODO")]
        public bool EmploymentPlacementWaitlistIsOpen { get; set; }

        public override void CopyStaticValues<T>(T data)
        {
            var d = data as EmploymentData;

            this.JobReadinessTrainingTotal = d.JobReadinessTrainingTotal;
            this.PaidInternshipTotal = d.PaidInternshipTotal;
            this.VocationalTrainingTotal = d.VocationalTrainingTotal;
            this.EmploymentPlacementTotal = d.EmploymentPlacementTotal;

            this.JobReadinessTrainingHasWaitlist = d.JobReadinessTrainingHasWaitlist;
            this.PaidInternshipHasWaitlist = d.PaidInternshipHasWaitlist;
            this.VocationalTrainingHasWaitlist = d.VocationalTrainingHasWaitlist;
            this.EmploymentPlacementHasWaitlist = d.EmploymentPlacementHasWaitlist;

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
