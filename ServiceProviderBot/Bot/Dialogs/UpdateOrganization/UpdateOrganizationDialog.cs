using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization
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
                    var needsUpdate = await NeedsUpdate(state, stepContext.Context, cancellationToken);
                    if (!needsUpdate)
                    {
                        // Nothing to update.
                        await Utils.Messages.SendAsync(Phrases.UpdateOrganization.NothingToUpdate, stepContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Push the capacity dialog onto the stack.
                    return await stepContext.BeginDialogAsync(UpdateCapacityDialog.Name, null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Send the closing message.
                    await Utils.Messages.SendAsync(Phrases.UpdateOrganization.Closing, stepContext.Context, cancellationToken);

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }

        private static async Task<bool> NeedsUpdate(StateAccessors state, ITurnContext context, CancellationToken cancellationToken)
        {
            var profile = await state.GetOrganizationProfile(context, cancellationToken);

            // Currently the only thing to update is the beds.
            return profile.Capacity.Beds.Total > 0;
        }
    }
}
