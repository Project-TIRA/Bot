using EntityModel;
using Shared;
using System.Collections.Generic;

namespace SearchBot.Bot.Models
{
    public class MatchData
    {
        private const int REASONABLE_DISTANCE = 25;

        public Organization Organization { get; set; }
        public List<ServiceType> OrganizationServiceTypes { get; set; }
        public ServiceFlags OrganizationServiceFlags { get; set; }

        public ServiceFlags RequestedServiceFlags { get; set; }

        public double Distance { get; set; }

        public bool IsFullMatch { get { return this.OrganizationServiceFlags.HasFlag(this.RequestedServiceFlags); } }
        public bool IsWithinDistance {  get { return this.Distance <= REASONABLE_DISTANCE; } }
        public string ServicesString { get { return Helpers.GetServicesString(this.OrganizationServiceTypes); } }

        public MatchData()
        {
            this.OrganizationServiceTypes = new List<ServiceType>();
        }
    }
}
