using EntityModel;
using System;

namespace SearchBot.Bot.Models
{
    public class ServiceContext
    {
        public ServiceType ServiceType { get; set; }
        public ServiceFlags ServiceFlags { get; set; }

        public bool IsValid { get { return this.ServiceFlags != ServiceFlags.None; } }

        public ServiceContext(ServiceType serviceType, ServiceFlags serviceFlags)
        {
            this.ServiceType = serviceType;
            this.ServiceFlags = serviceFlags;
        }
    }

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
