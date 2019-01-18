using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Models;

namespace TestBot.Bot.Dialogs.NewOrganization
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
                    // Prompt for the age range.
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = Utils.Phrases.NewOrganization.GetHasDemographicAgeRange
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Push the age range dialog onto the stack.
                        return await stepContext.BeginDialogAsync(DemographicDialog.Name, null, cancellationToken);
                    }

                    // Update the profile with the default age range.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Demographic.AgeRange.SetToAll();

                    // Prompt for working with men.
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = Utils.Phrases.NewOrganization.GetHasDemographicMen
                    },
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
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = Utils.Phrases.NewOrganization.GetHasDemographicWomen
                    },
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

                    // Push the age range dialog onto the stack.
                    return await stepContext.BeginDialogAsync(AgeRangeDialog.Name, null, cancellationToken);
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
