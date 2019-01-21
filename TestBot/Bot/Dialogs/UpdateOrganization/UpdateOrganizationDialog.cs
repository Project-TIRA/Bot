using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Dialogs.UpdateOrganization.Capacity;

namespace TestBot.Bot.Dialogs.UpdateOrganization
{
    public static class UpdateOrganizationDialog
    {
        public static string Name = nameof(UpdateOrganizationDialog);

        /// <summary>Creates a dialog for updating an organization.</summary>
        /// <param name="state">The state accessors.</param>
        public static Dialog Create(StateAccessors state)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Push the capacity dialog onto the stack.
                    return await stepContext.BeginDialogAsync(UpdateCapacityDialog.Name, null, cancellationToken);
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
