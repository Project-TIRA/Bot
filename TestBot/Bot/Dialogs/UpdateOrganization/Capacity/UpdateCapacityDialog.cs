using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Dialogs.Shared;

namespace TestBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public static class UpdateCapacityDialog
    {
        public static string Name = nameof(UpdateCapacityDialog);

        /// <summary>Creates a dialog for updating capacity.</summary>
        /// <param name="state">The state accessors.</param>
        public static Dialog Create(StateAccessors state)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Check if the organization has housing.
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    if (profile.Capacity.Beds.Total > 0)
                    {
                        // Push the update housing dialog onto the stack.
                        return await stepContext.BeginDialogAsync(HousingDialog.Name, null, cancellationToken);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
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
