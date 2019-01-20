using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using TestBot.Bot.Dialogs.NewOrganization;
using TestBot.Bot.Prompts;

namespace TestBot.Bot.Dialogs
{
    public class MasterDialog : DialogSet
    {
        public static string Name = "MasterDialog";

        /// <summary>Creates a new instance of this dialog set.</summary>
        /// <param name="state">The state accessors.</param>
        public MasterDialog(StateAccessors state) : base(state.DialogContextAccessor)
        {
            // Register dialogs and prompts.
            Utils.Dialogs.Register(this, state);
            Utils.Prompts.Register(this);

            // Define the dialog and add it to the set.
            Add(new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the action.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ChoicePrompt,
                        new WelcomeChoicePrompt(),
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var choice = (FoundChoice)stepContext.Result;

                    // Branch based on the input.
                    switch (choice.Index)
                    {
                        case WelcomeChoicePrompt.NewOrganizationChoice:
                        {
                            // Push the new organization dialog onto the stack.
                            return await stepContext.BeginDialogAsync(NewOrganizationDialog.Name, null, cancellationToken);
                        }
                        default:
                        {
                            return await stepContext.NextAsync(null, cancellationToken);
                        }
                    }
                },
                async (stepContext, cancellationToken) =>
                {
                    // Send the closing message.
                    await Utils.Messages.SendAsync(Utils.Phrases.Greeting.GetClosing, stepContext.Context, cancellationToken);

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            }));
        }
    }
}
