using EntityModel;
using System.Collections.Generic;

namespace Shared.Models
{
    public class MatchData
    {
        private const int REASONABLE_DISTANCE = 25;

        public Organization Organization { get; set; }
        public ServiceFlags OrganizationServiceFlags { get; set; }

        public ServiceFlags RequestedServiceFlags { get; set; }

        public double Distance { get; set; }

        public bool IsFullMatch { get { return this.OrganizationServiceFlags.HasFlag(this.RequestedServiceFlags); } }
        public bool IsWithinDistance {  get { return this.Distance <= REASONABLE_DISTANCE; } }
        public List<ServiceData> OrganizationDataTypes
        {
            get
            {
                var types = new List<ServiceData>();

                foreach (var type in Helpers.GetServiceDataTypes())
                {
                    foreach (var subService in type.SubServices())
                    {
                        if (this.OrganizationServiceFlags.HasFlag(subService.ServiceFlag))
                        {
                            types.Add(type);
                            break;
                        }
                    }
                }

                return types;
            }
        }
    }
}
