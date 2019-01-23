using EntityModel;
using Microsoft.Bot.Builder.Dialogs;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCapacityDialog : DialogBase
    {
        public static string Name = typeof(UpdateCapacityDialog).FullName;

        public override WaterfallDialog Init(DbModel dbContext, StateAccessors state, DialogSet dialogs)
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
                        return await Utils.Dialogs.BeginDialogAsync(dbContext, state, dialogs, stepContext, UpdateHousingDialog.Name, null, cancellationToken);
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
