using EntityModel;
using System.Collections.Generic;

namespace SearchBot.Bot.Models
{
    public class OrganizationMatch
    {
        private const int REASONABLE_DISTANCE = 25;

        public Organization Organization { get; set; }
        public List<ServiceDataBase> ServiceData { get; set; }
        public ServiceFlags ServiceFlags { get; set; }

        public double Distance { get; set; }

        public bool AllServicesMatch { get { return this.ServiceFlags == ServiceFlags.All; } }
        public bool SomeServicesMatch { get { return this.ServiceFlags != ServiceFlags.None; } }

        public bool IsWithinDistance()
        {
            return this.Distance <= REASONABLE_DISTANCE;
        }
    }
}
