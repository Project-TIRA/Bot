using EntityModel;
using System;
using System.Collections.Generic;

namespace Shared.Models
{
    [Flags]
    public enum ServiceFlags
    {
        None = 0,

        CaseManagement = 1 << 0,
        Employment = 1 << 1,
        EmploymentInternship = 1 << 2,
        HousingEmergency = 1 << 3,
        HousingLongTerm = 1 << 4,
        MentalHealth = 1 << 5,
        SubstanceUse = 1 << 6,
        SubstanceUseDetox = 1 << 7,

        All = ~(~0 << 8),

        EmploymentAll = Employment | EmploymentInternship,
        HousingAll = HousingEmergency | HousingLongTerm,
        SubstanceUseAll = SubstanceUse | SubstanceUseDetox
    }

    public static class ServiceFlagsExtensions
    {
        public static IEnumerable<ServiceFlags> GetFlags(this ServiceFlags flags)
        {
            foreach (ServiceFlags flag in Enum.GetValues(flags.GetType()))
            {
                if (flags.HasFlag(flag) && flag > ServiceFlags.None && flag < ServiceFlags.All)
                {
                    yield return flag;
                }
            }
        }

        public static ServiceType ToServiceType(this ServiceFlags flags)
        {
            if (ServiceFlags.CaseManagement.HasFlag(flags))
            {
                return ServiceType.CaseManagement;
            }
            else if (ServiceFlags.EmploymentAll.HasFlag(flags))
            {
                return ServiceType.Employment;
            }
            else if (ServiceFlags.HousingAll.HasFlag(flags))
            {
                return ServiceType.Housing;
            }
            else if (ServiceFlags.MentalHealth.HasFlag(flags))
            {
                return ServiceType.MentalHealth;
            }
            else if (ServiceFlags.SubstanceUseAll.HasFlag(flags))
            {
                return ServiceType.SubstanceUse;
            }

            return ServiceType.Invalid;
        }
    }
}
