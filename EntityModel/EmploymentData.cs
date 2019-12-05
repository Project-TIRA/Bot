using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace EntityModel
{
    public class EmploymentData : ServiceData
    {
        public const string TABLE_NAME = "TODO";
        public const string PRIMARY_KEY = "TODO";
        public const string SERVICE_NAME = "Employment";

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

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }
        public override string PrimaryKey() { return PRIMARY_KEY; }
        public override ServiceType ServiceType() { return EntityModel.ServiceType.Employment; }
        public override string ServiceTypeName() { return SERVICE_NAME; }

        public override List<UpdateSteps> UpdateSteps()
        {
            return new List<UpdateSteps>()
            {
                new UpdateSteps()
                {
                    Name = "Job Readiness Training",
                    TotalPropertyName = nameof(this.JobReadinessTrainingTotal),
                    OpenPropertyName = nameof(this.JobReadinessTrainingOpen),
                    HasWaitlistPropertyName = nameof(this.JobReadinessTrainingHasWaitlist),
                    WaitlistIsOpenPropertyName = nameof(this.JobReadinessTrainingWaitlistIsOpen)
                },
                new UpdateSteps()
                {
                    Name = "Paid Internships",
                    TotalPropertyName = nameof(this.PaidInternshipTotal),
                    OpenPropertyName = nameof(this.PaidInternshipOpen),
                    HasWaitlistPropertyName = nameof(this.PaidInternshipHasWaitlist),
                    WaitlistIsOpenPropertyName = nameof(this.PaidInternshipWaitlistIsOpen)
                },
                new UpdateSteps()
                {
                    Name = "Vocational Training",
                    TotalPropertyName = nameof(this.VocationalTrainingTotal),
                    OpenPropertyName = nameof(this.VocationalTrainingOpen),
                    HasWaitlistPropertyName = nameof(this.VocationalTrainingHasWaitlist),
                    WaitlistIsOpenPropertyName = nameof(this.VocationalTrainingWaitlistIsOpen)
                },
                new UpdateSteps()
                {
                    Name = "Employment Placement",
                    TotalPropertyName = nameof(this.EmploymentPlacementTotal),
                    OpenPropertyName = nameof(this.EmploymentPlacementOpen),
                    HasWaitlistPropertyName = nameof(this.EmploymentPlacementHasWaitlist),
                    WaitlistIsOpenPropertyName = nameof(this.EmploymentPlacementWaitlistIsOpen)
                }
            };
        }

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
