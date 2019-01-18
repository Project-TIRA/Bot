using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Utils;
using TestBot.Bot.Models;

namespace TestBot.Bot.Dialogs.NewOrganization
{
    public static class NewOrganizationDialog
    {
        public static string Name = "NewOrganizationDialog";

        /// <summary>Creates a dialog for adding a new organization.</summary>
        /// <param name="state">The state accessors.</param>
        public static Dialog Create(StateAccessors state)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the name.
                    return await stepContext.PromptAsync(Utils.Prompts.TextPrompt, new PromptOptions
                    {
                        Prompt = Phrases.NewOrganization.GetName
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the name.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Name = (string)stepContext.Result;

                    // Prompt for the demographics.
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = Utils.Phrases.NewOrganization.GetHasDemographic
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Push the demographics dialog onto the stack.
                        return await stepContext.BeginDialogAsync(DemographicDialog.Name, null, cancellationToken);
                    }

                    // Update the profile with the default demographics.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Demographic.SetToAll();

                    // Push the capacity dialog onto the stack.
                    return await stepContext.BeginDialogAsync(CapacityDialog.Name, null, cancellationToken);
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
