using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }
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
        public ICollection<OrganizationSnapshot> Snapshots { get; set; }
    }
}
