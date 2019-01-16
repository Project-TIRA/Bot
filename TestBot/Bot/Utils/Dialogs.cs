using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Dialogs;
using TestBot.Bot.Dialogs.NewOrg;

namespace TestBot.Bot.Utils
{
    public static class Dialogs
    {
        /// <summary>
        /// Adds each dialog to the global dialog set
        /// </summary>
        public static void Init(Accessors accessors, DialogSet globalDialogSet)
        {
            var ageRange = new AgeRangeDialog(accessors, globalDialogSet);
            var demographics = new DemographicsDialog(accessors, globalDialogSet);
            var newOrg = new NewOrgDialog(accessors, globalDialogSet);
            var updateOrg = new UpdateOrgDialog(accessors, globalDialogSet);
        }
    }
}
