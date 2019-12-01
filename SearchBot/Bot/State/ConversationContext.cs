using EntityModel;
using Luis;
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
        public List<ServiceContext> ServiceContexts { get; set; }
        public ServiceFlags ServiceFlags { get; set; }

        // These setters must be public for initializing conversation
        // state, but SetLocation() should be used to set location and position.
        public string Location { get; set; }
        public LocationPosition LocationPosition { get; set; }

        public bool HasServices { get { return this.ServiceContexts.Count > 0; } }

        public ConversationContext()
        {
            this.ServiceContexts = new List<ServiceContext>();
        }

        public override int GetHashCode()
        {
            return this.ServiceFlags.GetHashCode() ^
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
            return ctx.ServiceFlags == this.ServiceFlags && ctx.Location == this.Location;
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

            if (luisModel.Entities?.CaseManangement != null && luisModel.Entities.CaseManangement.Length > 0)
            {
                CreateOrUpdateServiceContext(ServiceType.CaseManagement, ServiceFlags.CaseManagement);
            }

            if (luisModel.Entities?.Employment != null && luisModel.Entities.Employment.Length > 0)
            {
                CreateOrUpdateServiceContext(ServiceType.Employment, ServiceFlags.Employment);
            }

            if (luisModel.Entities?.EmploymentInternship != null && luisModel.Entities.EmploymentInternship.Length > 0)
            {
                CreateOrUpdateServiceContext(ServiceType.Employment, ServiceFlags.EmploymentInternship);
            }

            if (luisModel.Entities?.Housing != null && luisModel.Entities.Housing.Length > 0)
            {
                // If no specific type of housing was requested then it needs to be clarified.
                CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);
            }

            if (luisModel.Entities?.HousingEmergency != null && luisModel.Entities.HousingEmergency.Length > 0)
            {
                CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.HousingEmergency);
            }

            if (luisModel.Entities?.HousingLongTerm != null && luisModel.Entities.HousingLongTerm.Length > 0)
            {
                CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.HousingLongTerm);
            }

            if (luisModel.Entities?.MentalHealth != null && luisModel.Entities.MentalHealth.Length > 0)
            {
                CreateOrUpdateServiceContext(ServiceType.MentalHealth, ServiceFlags.MentalHealth);
            }

            if (luisModel.Entities?.SubstanceUse != null && luisModel.Entities.SubstanceUse.Length > 0)
            {
                CreateOrUpdateServiceContext(ServiceType.SubstanceUse, ServiceFlags.SubstanceUse);
            }

            if (luisModel.Entities?.SubstanceUseDetox != null && luisModel.Entities.SubstanceUseDetox.Length > 0)
            {
                CreateOrUpdateServiceContext(ServiceType.SubstanceUse, ServiceFlags.SubstanceUseDetox);
            }
        }

        public void CreateOrUpdateServiceContext(ServiceType serviceType, ServiceFlags serviceFlags)
        {
            var context = this.ServiceContexts.FirstOrDefault(c => c.ServiceType == serviceType);
            if (context != null)
            {
                context.ServiceFlags |= serviceFlags;
            }
            else
            {
                this.ServiceContexts.Add(new ServiceContext(serviceType, serviceFlags));
            }

            this.ServiceFlags |= serviceFlags;
        }

        public bool HasService(ServiceType serviceType)
        {
            return this.ServiceContexts.FirstOrDefault(c => c.ServiceType == serviceType) != null;
        }

        public bool IsServiceInvalid(ServiceType serviceType)
        {
            var context = this.ServiceContexts.FirstOrDefault(c => c.ServiceType == serviceType);
            return context != null && !context.IsValid;
        }

        public List<ServiceType> GetServiceTypes()
        {
            var serviceTypes = new List<ServiceType>();
            this.ServiceContexts.ForEach(c => serviceTypes.Add(c.ServiceType));
            return serviceTypes;
        }

        public bool IsValid()
        {
            bool isValid = true;

            // Must have location.
            isValid &= !string.IsNullOrEmpty(this.Location);

            // All service contexts must be valid.
            this.ServiceContexts.ForEach(c => isValid &= c.IsValid);

            return isValid;
        }

        public void TEST_SetLocation(string location, LocationPosition position)
        {
            this.Location = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(location);
            this.LocationPosition = position;
        }
    }
}