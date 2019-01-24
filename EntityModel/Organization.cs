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
        public int TotalBeds { get; set; }

        // Snapshots
        public ICollection<Snapshot> Snapshots { get; set; }

        public Organization()
        {
            this.Id = Guid.NewGuid();
            this.DateCreated = DateTime.UtcNow;
            this.IsVerified = false;

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

        public void SetDefaultAgeRange()
        {
            this.AgeRangeStart = 0;
            this.AgeRangeEnd = 0;
        }
    }
}
