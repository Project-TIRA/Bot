using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.CaseManagement
{
    public class CaseManagementAgeRangeDialog : DialogBase
    {
        public static string Name = typeof(CaseManagementAgeRangeDialog).FullName;

        public CaseManagementAgeRangeDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the youngest age.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.CaseManagement.GetAgeRangeStart },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the youngest age.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.CaseManagementAgeRangeStart = (int)stepContext.Result;
                    await database.Save();

                    // Prompt for the oldest age.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.CaseManagement.GetAgeRangeEnd },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);

                    // Validate the numbers.
                    var end = (int)stepContext.Result;
                    if (end < organization.CaseManagementAgeRangeStart)
                    {
                        organization.SetDefaultAgeRange();
                        await database.Save();

                        // Send error message.
                        await Messages.SendAsync(Phrases.AgeRange.GetAgeRangeError, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the oldest age.
                    organization.CaseManagementAgeRangeEnd = (int)stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
