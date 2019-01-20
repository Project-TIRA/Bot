using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Models;

namespace TestBot.Bot.Dialogs.Shared.Capacity
{
    public static class HousingDialog
    {
        public static string Name = "HousingDialog";

        /// <summary>Creates a dialog for getting capacity.</summary>
        /// <param name="state">The state accessors.</param>
        public static Dialog Create(StateAccessors state)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the total beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Utils.Phrases.Shared.GetHousingTotal },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the total beds.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Capacity.Beds.Total = (int)stepContext.Result;

                    // Prompt for the open beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Utils.Phrases.Shared.GetHousingOpen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    // Validate the numbers.
                    var open = (int)stepContext.Result;
                    if (open > profile.Capacity.Beds.Total)
                    {
                        profile.Capacity.Beds.SetToNone();

                        // Send error message.
                        await Utils.Messages.SendAsync(Utils.Phrases.Shared.GetHousingError, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the open beds.
                    profile.Capacity.Beds.Open = (int)stepContext.Result;

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
