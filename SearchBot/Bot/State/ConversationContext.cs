using EntityModel;
using EntityModel.Luis;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.Models;
using Shared;
using Shared.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SearchBot.Bot.State
{
    public class ConversationContext
    {
        public List<ServiceContext> RequestedServices { get; set; }

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
            int hashCode = this.Location.GetHashCode() ^
                this.LocationPosition.GetHashCode() ^
                this.RequestedServiceFlags().GetHashCode();

            this.RequestedServices.ForEach(s => hashCode ^= s.GetHashCode());
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            ConversationContext ctx = (ConversationContext)obj;
            return ctx.Location == this.Location &&
                ctx.RequestedServiceFlags() == this.RequestedServiceFlags();
        }

        public static bool operator ==(ConversationContext c1, ConversationContext c2)
        {
            return Equals(c1, c2);
        }

        public static bool operator !=(ConversationContext c1, ConversationContext c2)
        {
            return !Equals(c1, c2);
        }

        public static (ServiceData DataType, LuisMapping Mapping) GetLuisMapping(string entityName)
        {
            foreach (var type in Helpers.GetServiceDataTypes())
            {
                var mapping = type.LuisMappings().FirstOrDefault(m => m.EntityName == entityName);
                if (mapping != null)
                {
                    return (type, mapping);
                }
            }

            Debug.Assert(false);
            return (null, null);
        }

        public ServiceFlags RequestedServiceFlags()
        {
            return this.RequestedServices.Select(s => s.RequestedServiceFlags).Aggregate(ServiceFlags.None, (f1, f2) => f1 |= f2);
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

            // Go through each LUIS entity and check if any service type handles it.
            foreach (var entity in luisModel.Entities.GetType().GetFields())
            {
                var value = entity.GetValue(luisModel.Entities) as string[];
                if (value == null || value.Length == 0)
                {
                    continue;
                }

                var (type, mapping) = GetLuisMapping(entity.Name);
                CreateServiceContext(type, mapping);
            }
        }

        public void CreateServiceContext(ServiceData dataType, LuisMapping luisMapping)
        {
            CreateOrUpdateServiceContext(dataType, luisMapping.ServiceFlags);
        }

        public void CreateOrUpdateServiceContext(ServiceData dataType, ServiceFlags requestedFlags)
        {
            var context = this.RequestedServices.FirstOrDefault(c => c.ServiceType == dataType.ServiceType());

            if (context != null)
            {
                context.RequestedServiceFlags |= requestedFlags;
            }
            else
            {
                this.RequestedServices.Add(new ServiceContext(dataType.ServiceType(), requestedFlags));
            }
        }

        public List<ServiceData> GetRequestedDataTypes()
        {
            return this.RequestedServices.Select(s => s.DataType()).ToList();
        }

        public bool HasRequestedDataType(ServiceData dataType)
        {
            return this.RequestedServices.FirstOrDefault(c => c.ServiceType == dataType.ServiceType()) != null;
        }

        public bool IsDataTypeComplete(ServiceData dataType)
        {
            var context = this.RequestedServices.FirstOrDefault(c => c.ServiceType == dataType.ServiceType());
            return context != null && context.IsComplete();
        }

        public bool IsComplete()
        {
            bool isValid = true;

            // Must have location.
            isValid &= !string.IsNullOrEmpty(this.Location);

            // All service contexts must be complete.
            this.RequestedServices.ForEach(c => isValid &= c.IsComplete());

            return isValid;
        }

        public void TEST_SetLocation(string location, LocationPosition position)
        {
            this.Location = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(location);
            this.LocationPosition = position;
        }
    }
}