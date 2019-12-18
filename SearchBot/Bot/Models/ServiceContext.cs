using EntityModel;
using EntityModel.Helpers;
using Shared;

namespace SearchBot.Bot.Models
{
    public class ServiceContext
    {
        public ServiceType ServiceType { get; set; }
        public ServiceFlags RequestedServiceFlags { get; set; }

        public ServiceContext(ServiceType serviceType, ServiceFlags requestedServiceFlags)
        {
            this.ServiceType = serviceType;
            this.RequestedServiceFlags = requestedServiceFlags;
        }

        public ServiceData DataType()
        {
            return Helpers.GetServiceDataTypeByServiceType(this.ServiceType);
        }

        public bool IsComplete()
        {
            return this.RequestedServiceFlags != ServiceFlags.None;
        }
    }
}
