﻿using System.Threading;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization
{
    public class UpdateOrganizationDialog : DialogBase
    {
        public static string Name = typeof(UpdateOrganizationDialog).FullName;

        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var needsUpdate = await NeedsUpdate(state, stepContext.Context);
                    if (!needsUpdate)
                    {
                        // Nothing to update.
                        await Messages.SendAsync(Phrases.UpdateOrganization.NothingToUpdate, stepContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Create a new snapshot to be filled in by UpdateOrganization process.
                    await state.CreateSnapshot(stepContext.Context);

                    // Push the update capacity dialog onto the stack.
                    return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, UpdateCapacityDialog.Name, null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Send the closing message.
                    await Messages.SendAsync(Phrases.UpdateOrganization.Closing, stepContext.Context, cancellationToken);

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }

        private static async Task<bool> NeedsUpdate(StateAccessors state, ITurnContext context)
        {
            var organization = await state.GetOrganization(context);

            // Currently the only thing to update is the beds.
            return organization.TotalBeds > 0;
        }
    }
}
