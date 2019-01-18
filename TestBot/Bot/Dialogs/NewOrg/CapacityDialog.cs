using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Dialogs.NewOrg.Capacity;

namespace TestBot.Bot.Dialogs.NewOrg
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
                    // Push the housing dialog onto the stack.
                    return await stepContext.BeginDialogAsync(HousingDialog.Name, null, cancellationToken);
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
