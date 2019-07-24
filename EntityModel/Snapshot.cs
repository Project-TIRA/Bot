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
        public int OpenBeds { get; set; }

        // Mental Health 
        public int MentalHealth_InPatientOpen { get; set; }
        public int MentalHealth_InPatientWaitlistLength { get; set; }
        public int MentalHealth_OutPatientOpen { get; set; }
        public int MentalHealth_OutPatientWaitlistLength { get; set; }

        public Snapshot(Guid organizationId)
        {
            this.Id = Guid.NewGuid();
            this.OrganizationId = organizationId;
            this.Date = DateTime.UtcNow;
        }
    }
}
