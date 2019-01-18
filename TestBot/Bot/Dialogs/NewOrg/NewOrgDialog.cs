using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Utils;
using TestBot.Bot.Models;

namespace TestBot.Bot.Dialogs.NewOrg
{
    public static class NewOrgDialog
    {
        public static string Name = "NewOrgDialog";

        /// <summary>Creates a dialog for adding a new organization.</summary>
        /// <param name="state">The state accessors.</param>
        public static Dialog Create(StateAccessors state)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    return await stepContext.PromptAsync(Utils.Prompts.TextPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What is the name of your organization?")
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the result of the previous step.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Name = (string)stepContext.Result;

                    // Push the demographics dialog onto the stack.
                    return await stepContext.BeginDialogAsync(DemographicsDialog.Name, null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
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
