using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Dialogs;
using TestBot.Bot.Dialogs.NewOrg;

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
            dialogs.Add(DemographicsDialog.Create(accessors));
            dialogs.Add(NewOrgDialog.Create(accessors));
        }
    }
}
