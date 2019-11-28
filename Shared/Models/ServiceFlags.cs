using System;

namespace Shared.Models
{
    [Flags]
    public enum ServiceFlags
    {
        None = 0,

        CaseManagement = 1 << 0,
        HousingEmergency = 1 << 1,
        HousingLongTerm = 1 << 2,
        Employment = 1 << 3,
        EmploymentInternship = 1 << 4,
        MentalHealth = 1 << 5,
        SubstanceUse = 1 << 6,
        SubstanceUseDetox = 1 << 7,

        All = ~(~0 << 8)
    }
}
