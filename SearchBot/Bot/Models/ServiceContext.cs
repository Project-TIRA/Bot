using EntityModel;
using Shared.Models;

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
}
