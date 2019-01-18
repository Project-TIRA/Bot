using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Dialogs.NewOrganization;

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
                    // Kick off the first dialog.
                    // TODO: This will branch based on the conversation (new org, update existing, etc.)
                    return await stepContext.BeginDialogAsync(NewOrganizationDialog.Name, null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            }));
        }
    }
}
