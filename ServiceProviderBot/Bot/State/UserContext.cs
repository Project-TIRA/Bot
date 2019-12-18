using EntityModel.Helpers;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.State
{
    public class UserContext
    {
        public string UserId { get; set; }
        public string OrganizationId { get; set; }
        public int TimezoneOffset { get; set; }
        public List<ServiceType> TypesToUpdate { get; set; }

        public UserContext()
        {
            this.TypesToUpdate = new List<ServiceType>();
        }
    }
}