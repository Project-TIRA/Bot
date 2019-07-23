using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public class Organization
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        // Verification
        public bool IsComplete { get; set; }
        public bool IsVerified { get; set; }

        // Location
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        // Demographics
        public Gender Gender { get; set; }
        public int AgeRangeStart { get; set; }
        public int AgeRangeEnd { get; set; }

        // Capacity
        public Frequency UpdateFrequency { get; set; }
        public int TotalBeds { get; set; }

        // Case Management
        public int CaseManagementTotal { get; set; }
        public bool CaseManagementHasWaitlist { get; set; }
        public Gender CaseManagementGender { get; set; }
        public int CaseManagementAgeRangeStart { get; set; }
        public int CaseManagementAgeRangeEnd { get; set; }
        public bool CaseManagementSobriety { get; set; }

        // Snapshots
        public ICollection<Snapshot> Snapshots { get; set; }

        // Job Training Services
        public bool HasJobTrainingServices { get; set; }
        public bool HasJobTrainingWaitlist { get; set; }
        public int TotalJobTrainingPositions { get; set; }
        public int OpenJobTrainingPositions { get; set; }
        public int JobTrainingWaitlistPositions { get; set; }

        public Organization()
        {
            this.Id = Guid.NewGuid();
            this.DateCreated = DateTime.UtcNow;
            this.IsVerified = false;
            this.UpdateFrequency = Frequency.Weekly;

            this.Snapshots = new List<Snapshot>();
        }

        public void UpdateGender(Gender gender, bool add)
        {
            if (add)
            {
                this.Gender |= gender;
            }
            else
            {
                this.Gender &= ~gender;
            }
        }
        public void UpdateCaseManagementGender(Gender gender, bool add)
        {
            if (add)
            {
                this.CaseManagementGender |= gender;
            }
            else
            {
                this.CaseManagementGender &= ~gender;
            }
        }

        public void SetDefaultAgeRange()
        {
            this.AgeRangeStart = 0;
            this.AgeRangeEnd = 0;
        }

        public void SetDefaultAgeRangeCaseManagement()
        {
            this.CaseManagementAgeRangeStart = 0;
            this.CaseManagementAgeRangeEnd = 0;
        }
    }
}
