using System;

namespace ServiceProviderBot.Bot.Models.OrganizationProfile
{
    [Flags]
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
        All = Male | Female
        // Todo: Add more options
    }
}
