using System;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public class Snapshot
    {
        [Key]
        public Guid Id { get; set; }

        public bool IsComplete { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public DateTime Date { get; set; }
        public int BedsEmergencyPrivateOpen { get; set; }
        public int BedsEmergencyPrivateWaitlistLength { get; set; }
        public int BedsEmergencySharedOpen { get; set; }
        public int BedsEmergencySharedWaitlistLength { get; set; }
        public int BedsLongtermPrivateOpen { get; set; }
        public int BedsLongtermPrivateWaitlistLength { get; set; }
        public int BedsLongtermSharedOpen { get; set; }
        public int BedsLongtermSharedWaitlistLength { get; set; }

        public int OpenJobTrainingPositions { get; set; }

        public int JobTrainingWaitlistPositions { get; set; }

        //Case Management
        public int CaseManagementOpenSlots { get; set; }
        public int CaseManagementWaitlistLength { get; set; }

        public Snapshot(Guid organizationId)
        {
            this.Id = Guid.NewGuid();
            this.OrganizationId = organizationId;
            this.Date = DateTime.UtcNow;
        }
    }
}
