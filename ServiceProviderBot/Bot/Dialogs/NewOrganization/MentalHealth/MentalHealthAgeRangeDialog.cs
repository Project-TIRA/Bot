using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.MentalHealth
{
    public class MentalHealthAgeRangeDialog : DialogBase
    {
        public static string Name = typeof(MentalHealthAgeRangeDialog).FullName;

        public MentalHealthAgeRangeDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the youngest age for mental health
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.MentalHealth.GetAgeRangeStart },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the youngest age for mental health
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.MentalHealth_AgeRangeStart = (int)stepContext.Result; 
                    await database.Save();

                    // Prompt for the oldest age for mental health
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.MentalHealth.GetAgeRangeEnd },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);

                    // Validate the numbers
                    var end = (int)stepContext.Result;
                    if (end < organization.AgeRangeStart)
                    {
                        organization.SetMentalHealthDefaultAgeRange();
                        await database.Save();

                        // Send error message
                        await Messages.SendAsync(Phrases.MentalHealth.GetAgeRangeError, stepContext.Context, cancellationToken);

                        // Repeat the dialog
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the oldest age
                    organization.MentalHealth_AgeRangeEnd = (int)stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
