using EntityModel;
using Shared;
using System.Linq;

namespace SearchBot.Bot.Models
{
    public class ServiceContext
    {
        public ServiceType ServiceType { get; set; }
        public ServiceFlags ServiceFlags { get; set; }

        public ServiceContext(ServiceType serviceType, ServiceFlags serviceFlags)
        {
            this.ServiceType = serviceType;
            this.ServiceFlags = serviceFlags;
        }

        public bool IsValid()
        {
            var type = DataType();

            // Valid if there are no sub-categories or if one of the sub-categories is fulfilled.
            return type != null &&
                (type.SubServiceCategories().Count == 0 || type.SubServiceCategories().Any(c => this.ServiceFlags.HasFlag(c.ServiceFlag)));
        }

        public ServiceData DataType()
        {
            return Helpers.GetServiceByType(this.ServiceType);
        }
    }
}
