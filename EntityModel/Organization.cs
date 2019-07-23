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

        // Case Management
        public int CaseManagementTotal { get; set; }
        public bool CaseManagementHasWaitlist { get; set; }
        public Gender CaseManagementGender { get; set; }
        public int CaseManagementAgeRangeStart { get; set; }
        public int CaseManagementAgeRangeEnd { get; set; }
        public bool CaseManagementSobriety { get; set; }

        // Housing capacity
        public int HousingEmergencyPrivateTotal { get; set; }
        public int HousingEmergencySharedTotal { get; set; }
        public int HousingLongtermPrivateTotal { get; set; }
        public int HousingLongtermSharedTotal { get; set; }
        public bool HousingHasWaitlist { get; set; }
        
        // Housing eligibility
        public int HousingAgeRangeStart { get; set; }
        public int HousingAgeRangeEnd { get; set; }
        public Gender HousingGender { get; set; }
        public FamilyStatus HousingFamilyStatus { get; set; }
        public bool HousingServiceAnimal { get; set; }
        public bool HousingSobriety { get; set; }

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

        public void UpdateHousingGender(Gender gender, bool add)
        {
            if (add)
            {
                this.HousingGender |= gender;
            }
            else
            {
                this.HousingGender &= ~gender;
            }
        }

        public void UpdateHousingFamilyStatus(FamilyStatus familyStatus, bool add)
        {
            if (add)
            {
                this.HousingFamilyStatus |= familyStatus;
            }
            else
            {
                this.HousingFamilyStatus &= ~familyStatus;
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

        public void SetDefaultHousingAgeRange()
        {
            this.HousingAgeRangeStart = 0;
            this.HousingAgeRangeEnd = 0;
        }
    }
}
