using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Models;

namespace TestBot.Bot.Dialogs.NewOrg
{
    public static class DemographicsDialog
    {
        public static string Name = "DemographicsDialog";

        /// <summary>Creates a dialog for adding a new organization.</summary>
        /// <param name="accessors">The state accessors.</param>
        public static Dialog Create(StateAccessors accessors)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Does your organization work with men?")
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the result of the previous step.
                    var profile = await accessors.GetOrganizationProfile(stepContext.Context, cancellationToken);

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
                    var profile = await accessors.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    if ((bool)stepContext.Result)
                    {
                        profile.Demographic.Gender |= Gender.Female;
                    }
                    else
                    {
                         profile.Demographic.Gender &= ~Gender.Female;
                    }

                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Does your organization work with an age range?")
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
                    else
                    {
                        // Skip this step.
                        return await stepContext.NextAsync();
                    }
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
