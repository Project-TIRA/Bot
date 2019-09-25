using EntityModel;
using Luis;
using Shared;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SearchBot.Bot.State
{
    public class ConversationContext
    {
        public string Location { get; set; }

        public bool Housing { get; set; }
        public bool HousingEmergency { get; set; }
        public bool HousingLongTerm { get; set; }

        public bool Employment { get; set; }
        public bool EmploymentInternship { get; set; }

        private bool HasHousing { get { return this.Housing || this.HousingEmergency || this.HousingLongTerm; } }
        private bool HasEmployment { get { return this.Employment || this.EmploymentInternship; } }

        public override int GetHashCode()
        {
            return this.Location.GetHashCode() ^
                this.Housing.GetHashCode() ^ this.HousingEmergency.GetHashCode() ^ this.HousingLongTerm.GetHashCode() ^
                this.Employment.GetHashCode() ^ this.EmploymentInternship.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            ConversationContext ctx = (ConversationContext)obj;
            return ctx.Location == this.Location &&
                ctx.Housing == this.Housing &&
                ctx.HousingEmergency == this.HousingEmergency &&
                ctx.HousingLongTerm == this.HousingLongTerm &&
                ctx.Employment == this.Employment &&
                ctx.EmploymentInternship == this.EmploymentInternship;
        }

        public static bool operator ==(ConversationContext c1, ConversationContext c2)
        {
            return Object.Equals(c1, c2);
        }

        public static bool operator !=(ConversationContext c1, ConversationContext c2)
        {
            return !Object.Equals(c1, c2);
        }

        public void AddLuisResult(LuisModel luisModel)
        {
            if (luisModel.Entities?.geographyV2 != null && luisModel.Entities.geographyV2.Length > 0)
            {
                this.Location = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(luisModel.Entities.geographyV2[0].Location);
            }

            this.Housing = luisModel.Entities?.Housing != null && luisModel.Entities.Housing.Length > 0;
            this.HousingEmergency = luisModel.Entities?.HousingEmergency != null && luisModel.Entities.HousingEmergency.Length > 0;
            this.HousingLongTerm = luisModel.Entities?.HousingLongTerm != null && luisModel.Entities.HousingLongTerm.Length > 0;

            this.Employment = luisModel.Entities?.Employment != null && luisModel.Entities.Employment.Length > 0;
            this.EmploymentInternship = luisModel.Entities?.EmploymentInternship != null && luisModel.Entities.EmploymentInternship.Length > 0;
        }

        public List<ServiceType> GetServiceTypes()
        {
            var serviceTypes = new List<ServiceType>();

            foreach (ServiceType serviceType in Enum.GetValues(typeof(ServiceType)))
            {
                bool hasService = false;

                switch (serviceType)
                {
                    case ServiceType.CaseManagement: hasService = false; break;
                    case ServiceType.Housing: hasService = this.HasHousing; break;
                    case ServiceType.Employment: hasService = this.HasEmployment; break;
                    case ServiceType.MentalHealth: hasService = false; break;
                    case ServiceType.SubstanceUse: hasService = false; break;
                    default: hasService = false; break;
                }

                if (hasService)
                {
                    serviceTypes.Add(serviceType);
                }
            }

            return serviceTypes;
        }

        public string GetServicesString()
        {
            var serviceTypes = GetServiceTypes();

            if (serviceTypes.Count == 0)
            {
                return string.Empty;
            }
            else if (serviceTypes.Count == 1)
            {
                return Helpers.GetServiceName(serviceTypes[0]);
            }
            else if (serviceTypes.Count == 2)
            {
                return $"{Helpers.GetServiceName(serviceTypes[0])} and {Helpers.GetServiceName(serviceTypes[1])}";
            }
            else
            {
                string result = string.Empty;

                for (int i = 0; i < serviceTypes.Count; ++i)
                {
                    var separator = (i == serviceTypes.Count - 1) ? ", and " : (i > 0 ? ", " : string.Empty);
                    result += separator + Helpers.GetServiceName(serviceTypes[i]);
                }

                return result;
            }
        }

        public bool IsValid()
        {
            bool isValid = true;

            // Must have location.
            isValid &= !string.IsNullOrEmpty(this.Location);

            // Housing will be set if it wasn't clear which type of housing they are looking for.
            // Only once the specific type of housing is set is this service valid.
            isValid &= !this.Housing || this.HousingEmergency || this.HousingLongTerm;

            return isValid; 
        }
    }
}