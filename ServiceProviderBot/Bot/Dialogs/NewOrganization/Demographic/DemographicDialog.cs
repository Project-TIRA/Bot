using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Models.OrganizationProfile;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Demographic
{
    public class DemographicDialog : DialogBase
    {
        public static string Name = typeof(DemographicDialog).FullName;

        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
        {
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
                        return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, AgeRangeDialog.Name, null, cancellationToken);
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
