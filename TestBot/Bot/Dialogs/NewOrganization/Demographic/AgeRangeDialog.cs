﻿using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Dialogs.NewOrganization.Demographic
{
    public static class AgeRangeDialog
    {
        public static string Name = nameof(AgeRangeDialog);

        /// <summary>Creates a dialog for adding a new organization.</summary>
        /// <param name="state">The state accessors.</param>
        public static Dialog Create(StateAccessors state)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the youngest age.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Utils.Phrases.AgeRange.GetAgeRangeStart },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the youngest age.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Demographic.AgeRange.Start = (int)stepContext.Result;

                    // Prompt for the oldest age.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Utils.Phrases.AgeRange.GetAgeRangeEnd },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    // Validate the numbers.
                    var end = (int)stepContext.Result;
                    if (end < profile.Demographic.AgeRange.Start)
                    {
                        profile.Demographic.AgeRange.SetToAll();

                        // Send error message.
                        await Utils.Messages.SendAsync(Utils.Phrases.AgeRange.GetAgeRangeError, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the oldest age.
                    profile.Demographic.AgeRange.End = (int)stepContext.Result;

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}