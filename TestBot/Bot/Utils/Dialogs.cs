using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Dialogs.NewOrganization;
using TestBot.Bot.Dialogs.NewOrganization.Capacity;

namespace TestBot.Bot.Utils
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
            dialogs.Add(NewOrganizationDialog.Create(accessors));
        }
    }
}
