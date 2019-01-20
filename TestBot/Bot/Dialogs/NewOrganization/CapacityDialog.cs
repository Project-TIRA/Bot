using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Dialogs.Shared.Capacity;

namespace TestBot.Bot.Dialogs.NewOrganization
{
    public static class CapacityDialog
    {
        public static string Name = "CapacityDialog";

        /// <summary>Creates a dialog for getting capacity.</summary>
        /// <param name="state">The state accessors.</param>
        public static Dialog Create(StateAccessors state)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for housing capacity.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Utils.Phrases.NewOrganization.GetHasHousing },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Push the housing dialog onto the stack.
                        return await stepContext.BeginDialogAsync(HousingDialog.Name, null, cancellationToken);
                    }

                    // Update the profile with the default housing capacity.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Capacity.Beds.SetToNone();

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
