using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.CaseManagement
{
    public class UpdateCaseManagementDialog : DialogBase
    {
        public static string Name = typeof(UpdateCaseManagementDialog).FullName;

        public UpdateCaseManagementDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt to update number of open slots
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.CaseManagement.GetCaseManagementOpen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    var organization = await database.GetOrganization(stepContext.Context);

                    var openSlots = (int)stepContext.Result;
                    if(openSlots > organization.CaseManagementTotal)
                    {
                        // Send error message.
                        var error = string.Format(Phrases.CaseManagement.GetCaseManagementSpaceErrorFormat(organization.CaseManagementTotal));
                        await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }
                    snapshot.CaseManagementOpenSlots = openSlots;
                    await database.Save();

                    if(organization.CaseManagementHasWaitlist && openSlots == 0)
                    {
                        // Prompt to update waitlist length
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.CaseManagement.GetCaseManagementWaitlistLength },
                            cancellationToken);
                    }

                    snapshot.CaseManagementWaitlistLength = 0;
                    await database.Save();
                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    snapshot.CaseManagementWaitlistLength = (int)stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }

        public static async Task<bool> CanUpdate(StateAccessors state, DbInterface database, ITurnContext context)
        {
            var organization = await database.GetOrganization(context);
            
            // Updates valid 
            return organization.CaseManagementTotal > 0;
        }
    }
}
