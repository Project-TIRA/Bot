using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity
{
    public class HousingAgeRangeDialog : DialogBase
    {
        public static string Name = typeof(HousingAgeRangeDialog).FullName;

        public HousingAgeRangeDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
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
                        new PromptOptions { Prompt = Phrases.AgeRange.GetAgeRangeStart },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the youngest age.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.HousingAgeRangeStart = (int)stepContext.Result;
                    await database.Save();

                    // Prompt for the oldest age.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.AgeRange.GetAgeRangeEnd },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);

                    // Validate the numbers.
                    var end = (int)stepContext.Result;
                    if (end < organization.HousingAgeRangeStart)
                    {
                        organization.SetDefaultHousingAgeRange();
                        await database.Save();

                        // Send error message.
                        await Messages.SendAsync(Phrases.AgeRange.GetAgeRangeError, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the oldest age.
                    organization.HousingAgeRangeEnd = (int)stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
