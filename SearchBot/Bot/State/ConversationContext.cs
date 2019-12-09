using EntityModel;
using EntityModel.Luis;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.Models;
using Shared;
using Shared.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SearchBot.Bot.State
{
    public class ConversationContext
    {
        public List<ServiceContext> RequestedServices { get; set; }
        public ServiceFlags RequestedServiceFlags { get; set; }

        // These setters must be public for initializing conversation
        // state, but SetLocation() should be used to set location and position.
        public string Location { get; set; }
        public LocationPosition LocationPosition { get; set; }

        public bool HasRequestedServices { get { return this.RequestedServices.Count > 0; } }

        public ConversationContext()
        {
            this.RequestedServices = new List<ServiceContext>();
        }

        public override int GetHashCode()
        {
            return this.RequestedServiceFlags.GetHashCode() ^
                this.Location.GetHashCode() ^
                this.LocationPosition.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            ConversationContext ctx = (ConversationContext)obj;
            return ctx.RequestedServiceFlags == this.RequestedServiceFlags && ctx.Location == this.Location;
        }

        public static bool operator ==(ConversationContext c1, ConversationContext c2)
        {
            return Equals(c1, c2);
        }

        public static bool operator !=(ConversationContext c1, ConversationContext c2)
        {
            return !Equals(c1, c2);
        }

        public async Task SetLocation(IConfiguration configuration, string location)
        {
            this.LocationPosition = await Helpers.LocationToPosition(configuration, location);
            if (this.LocationPosition != null)
            {
                this.Location = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(location);
            }
        }

        public async Task AddLuisResult(IConfiguration configuration, LuisModel luisModel)
        {
            if (luisModel.Entities?.geographyV2 != null && luisModel.Entities.geographyV2.Length > 0)
            {
                await SetLocation(configuration, luisModel.Entities.geographyV2[0].Location);
            }

            // Go through each LUIS entities and check if any service type handles it.
            foreach (var entity in luisModel.Entities.GetType().GetFields())
            {
                var value = entity.GetValue(luisModel.Entities) as string[];
                if (value == null || value.Length == 0)
                {
                    continue;
                }

                var (type, flags) = LuisEntityToServiceTypeAndFlags(entity.Name);
                CreateOrUpdateServiceContext(type, flags);
            }
        }

        public void CreateOrUpdateServiceContext(ServiceData dataType, ServiceFlags serviceFlags)
        {
            var context = this.RequestedServices.FirstOrDefault(c => c.ServiceType == dataType.ServiceType());
            if (context != null)
            {
                context.ServiceFlags |= serviceFlags;
            }
            else
            {
                this.RequestedServices.Add(new ServiceContext(dataType.ServiceType(), serviceFlags));
            }

            this.RequestedServiceFlags |= serviceFlags;
        }

        public bool HasService(ServiceData dataType)
        {
            return this.RequestedServices.FirstOrDefault(c => c.ServiceType == dataType.ServiceType()) != null;
        }

        public bool IsServiceInvalid(ServiceData dataType)
        {
            var context = this.RequestedServices.FirstOrDefault(c => c.ServiceType == dataType.ServiceType());
            return context != null && !context.IsValid();
        }

        public List<ServiceData> GetServiceTypes()
        {
            return this.RequestedServices.Select(s => s.DataType()).ToList();
        }

        public bool IsValid()
        {
            bool isValid = true;

            // Must have location.
            isValid &= !string.IsNullOrEmpty(this.Location);

            // All service contexts must be valid.
            this.RequestedServices.ForEach(c => isValid &= c.IsValid());

            return isValid;
        }

        public void TEST_SetLocation(string location, LocationPosition position)
        {
            this.Location = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(location);
            this.LocationPosition = position;
        }

        private (ServiceData dataType, ServiceFlags ServiceFlags) LuisEntityToServiceTypeAndFlags(string entityName)
        {
            foreach (var type in Helpers.GetServiceDataTypes())
            {
                foreach (var subService in type.SubServices())
                {
                    if (subService.LuisEntityNames.Contains(entityName))
                    {
                        return (type, subService.ServiceFlag);
                    }
                }
            }

            return (null, ServiceFlags.None);
        }
    }
}