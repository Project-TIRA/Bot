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

            // Valid if any sub-service fulfills a requested service flag.
            return type != null && type.ServiceCategories().Any(c => c.Services.Any(s => this.ServiceFlags.HasFlag(s.ServiceFlags)));
        }

        public ServiceData DataType()
        {
            return Helpers.GetServiceByType(this.ServiceType);
        }
    }
}
