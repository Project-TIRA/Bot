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

        public int OpenJobTrainingPositions { get; set; }

        public int JobTrainingWaitlistPositions { get; set; }

        public Snapshot(Guid organizationId)
        {
            this.Id = Guid.NewGuid();
            this.OrganizationId = organizationId;
            this.Date = DateTime.UtcNow;
        }
    }
}
