using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.Dialogs.NewOrganization;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Demographic;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Location;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;

namespace ServiceProviderBot.Bot.Utils
{
    public static class Dialogs
    {
        /// <summary>
        /// Adds each dialog to the master dialog set
        /// </summary>
        public static void Register(DialogSet dialogs, StateAccessors accessors)
        {
            dialogs.Add(AgeRangeDialog.Create(accessors));
            dialogs.Add(HousingDialog.Create(accessors));
            dialogs.Add(CapacityDialog.Create(accessors));
            dialogs.Add(DemographicDialog.Create(accessors));
            dialogs.Add(LocationDialog.Create(accessors));
            dialogs.Add(MasterDialog.Create(accessors));
            dialogs.Add(NewOrganizationDialog.Create(accessors));
            dialogs.Add(UpdateOrganizationDialog.Create(accessors));
            dialogs.Add(UpdateCapacityDialog.Create(accessors));
            dialogs.Add(UpdateHousingDialog.Create(accessors));
        }
    }
}
