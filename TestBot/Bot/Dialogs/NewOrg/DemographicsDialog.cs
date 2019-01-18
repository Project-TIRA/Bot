using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Models;

namespace TestBot.Bot.Dialogs.NewOrg
{
    public static class DemographicsDialog
    {
        public static string Name = "DemographicsDialog";

        /// <summary>Creates a dialog for getting demographics.</summary>
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
                        Prompt = MessageFactory.Text("Does your organization work with a specific demographic?")
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (!(bool)stepContext.Result)
                    {
                        // Works with all demographics.
                        var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                        profile.Demographic.SetToAll();

                        // End this dialog to pop it off the stack.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Does your organization work with men?")
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the result of the previous step.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    if ((bool)stepContext.Result)
                    {
                        profile.Demographic.Gender |= Gender.Male;
                    }
                    else
                    {
                         profile.Demographic.Gender &= ~Gender.Male;
                    }

                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Does your organization work with women?")
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the result of the previous step.
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
