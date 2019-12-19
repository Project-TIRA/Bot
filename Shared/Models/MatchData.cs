using EntityModel;
using EntityModel.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Models
{
    public class MatchData
    {
        public Organization Organization { get; set; }
        public ServiceFlags OrganizationServiceFlags { get; set; }

        public ServiceFlags RequestedServiceFlags { get; set; }

        public double Distance { get; set; }

        public bool IsFullMatch { get { return this.OrganizationServiceFlags.HasFlag(this.RequestedServiceFlags); } }

        public List<ServiceData> OrganizationDataTypes()
        {
            var types = new List<ServiceData>();

            foreach (var flag in ServiceFlagsHelpers.SplitFlags(OrganizationServiceFlags))
            {
                var type = Helpers.ServiceFlagToDataType(flag);

                if (!types.Any(t => t.ServiceType() == type.ServiceType()))
                {
                    types.Add(Helpers.ServiceFlagToDataType(flag));
                }
            }

            return types;
        }
    }
}
