using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCapacityDialog : DialogBase
    {
        public static string Name = typeof(UpdateCapacityDialog).FullName;

        public UpdateCapacityDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Check if the organization has housing.
                    var organization = await database.GetOrganization(stepContext.Context);
                    if (organization.TotalBeds > 0)
                    {
                        // Push the update housing dialog onto the stack.
                        return await BeginDialogAsync(stepContext, UpdateHousingDialog.Name, null, cancellationToken);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Check if the organization has housing.
                    var organization = await database.GetOrganization(stepContext.Context);
                    if (organization.MentalHealth_InPatientTotal > 0 || organization.MentalHealth_OutPatientTotal > 0)
                    {
                        // Push the update housing dialog onto the stack.
                        return await BeginDialogAsync(stepContext, UpdateMentalHealthDialog.Name, null, cancellationToken);
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
