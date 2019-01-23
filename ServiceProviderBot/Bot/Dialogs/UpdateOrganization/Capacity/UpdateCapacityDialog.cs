using EntityModel;
using Microsoft.Bot.Builder.Dialogs;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCapacityDialog : DialogBase
    {
        public static string Name = typeof(UpdateCapacityDialog).FullName;

        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Check if the organization has housing.
                    var organization = await state.GetOrganization(stepContext.Context);
                    if (organization.TotalBeds > 0)
                    {
                        // Push the update housing dialog onto the stack.
                        return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, UpdateHousingDialog.Name, null, cancellationToken);
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
