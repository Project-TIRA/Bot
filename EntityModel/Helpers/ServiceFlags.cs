using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityModel.Helpers
{
    [Flags]
    public enum ServiceFlags : int
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
    }

    public static class ServiceFlagsHelpers
    {
        public static IEnumerable<ServiceFlags> AllFlags()
        {
            return Enum.GetValues(typeof(ServiceFlags)).OfType<ServiceFlags>().Where(f => f != ServiceFlags.None);
        }

        public static IEnumerable<ServiceFlags> SplitFlags(ServiceFlags flags)
        {
            foreach (ServiceFlags flag in AllFlags())
            {
                if (flags.HasFlag(flag))
                {
                    yield return flag;
                }
            }
        }
    }
}
