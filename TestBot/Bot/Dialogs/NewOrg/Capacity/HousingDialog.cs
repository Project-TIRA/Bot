using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Models;

namespace TestBot.Bot.Dialogs.NewOrg.Capacity
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
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Does your organization offer housing?")
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the result of the previous step.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    if (!(bool)stepContext.Result)
                    {
                        // Does not offer housing.
                        profile.Capacity.Beds.SetToNone();

                        // End this dialog to pop it off the stack.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    return await stepContext.PromptAsync(Utils.Prompts.IntPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("How many TOTAL beds does your organization have?")
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the result of the previous step.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Capacity.Beds.Total = (int)stepContext.Result;

                    return await stepContext.PromptAsync(Utils.Prompts.IntPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("How many OPEN beds does your organization have?")
                    },
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

                        // Repeat the dialog.
                        await Utils.Messages.SendAsync("Oops, the total beds must be greater than the open beds.", stepContext.Context, cancellationToken);
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);

                    }

                    // Update the profile with the result of the previous step.
                    profile.Capacity.Beds.Open = (int)stepContext.Result;

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
