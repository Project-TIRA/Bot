using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.CaseManagement
{
    public class CaseManagementDialog : DialogBase
    {
        public static string Name = typeof(CaseManagementDialog).FullName;

        public CaseManagementDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Ask if Case Management/Support Services are offered.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.CaseManagement.GetHasCaseManagement },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // If Case Management is not offered, set total capacity to zero and exit this dialog
                    if ((bool)stepContext.Result == false)
                    {
                        // Update the profile with the default capacity.
                        var organization = await database.GetOrganization(stepContext.Context);
                        organization.CaseManagementTotal = 0;
                        await database.Save();

                        // End this dialog to pop it off the stack.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Push the case management capacity dialog onto the stack.
                    return await BeginDialogAsync(stepContext, CaseManagementCapacityDialog.Name, null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Push the case management demographic dialog onto the stack.
                    return await BeginDialogAsync(stepContext, CaseManagementDemographDialog.Name, null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
