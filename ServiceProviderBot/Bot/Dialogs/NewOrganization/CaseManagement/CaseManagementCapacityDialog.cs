using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.CaseManagement
{
    public class CaseManagementCapacityDialog : DialogBase
    {
        public static string Name = typeof(CaseManagementCapacityDialog).FullName;

        public CaseManagementCapacityDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the total spots for case management
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.CaseManagement.GetCaseManagementTotal },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the total spots.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.CaseManagementTotal = (int)stepContext.Result;
                    await database.Save();

                    // Prompt for checking if waitlist available
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.CaseManagement.GetHasWaitingList },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with waitlist availability
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.CaseManagementHasWaitlist = (bool)stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
