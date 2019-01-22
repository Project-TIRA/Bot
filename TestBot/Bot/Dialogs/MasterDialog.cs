using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using TestBot.Bot.Dialogs.NewOrganization;
using TestBot.Bot.Dialogs.UpdateOrganization;
using TestBot.Bot.Prompts;

namespace TestBot.Bot.Dialogs
{
    public static class MasterDialog
    {
        public static string Name = nameof(MasterDialog);

        /// <summary>Creates a dialog for managing the conversation.</summary>
        /// <param name="state">The state accessors.</param>
        public static Dialog Create(StateAccessors state)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the action.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ChoicePrompt,
                        new GreetingPromptOptions(),
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var choice = (FoundChoice)stepContext.Result;

                    // Branch based on the input.
                    switch (choice.Index)
                    {
                        case GreetingPromptOptions.NewOrganizationChoice:
                        {
                            // Push the new organization dialog onto the stack.
                            return await stepContext.BeginDialogAsync(NewOrganizationDialog.Name, null, cancellationToken);
                        }
                        case GreetingPromptOptions.UpdateOrganizationChoice:
                        {
                            // Push the update organization dialog onto the stack.
                            return await stepContext.BeginDialogAsync(UpdateOrganizationDialog.Name, null, cancellationToken);
                        }
                        default:
                        {
                            return await stepContext.NextAsync(null, cancellationToken);
                        }
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
