using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Models;

namespace TestBot.Bot.Dialogs.Demographic
{
    public static class DemographicDialog
    {
        public static string Name = "DemographicDialog";

        /// <summary>Creates a dialog for getting demographics.</summary>
        /// <param name="state">The state accessors.</param>
        public static Dialog Create(StateAccessors state)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for working with men.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Utils.Phrases.Demographic.GetHasDemographicMen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the men demographic.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    if ((bool)stepContext.Result)
                    {
                        profile.Demographic.Gender |= Gender.Male;
                    }
                    else
                    {
                         profile.Demographic.Gender &= ~Gender.Male;
                    }

                    // Prompt for working with women.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Utils.Phrases.Demographic.GetHasDemographicWomen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the women demographic.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    if ((bool)stepContext.Result)
                    {
                        profile.Demographic.Gender |= Gender.Female;
                    }
                    else
                    {
                         profile.Demographic.Gender &= ~Gender.Female;
                    }

                    // Prompt for the age range.
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = Utils.Phrases.Demographic.GetHasDemographicAgeRange
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Push the age range dialog onto the stack.
                        return await stepContext.BeginDialogAsync(AgeRangeDialog.Name, null, cancellationToken);
                    }

                    // Update the profile with the default age range.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Demographic.AgeRange.SetToAll();

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
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
