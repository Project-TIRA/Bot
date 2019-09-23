using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCapacityDialog : DialogBase
    {
        public static string Name = typeof(UpdateCapacityDialog).FullName;

        public UpdateCapacityDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (dialogContext, cancellationToken) =>
                {
                    // Check if the organization has case management services.
                    var service = await api.GetService<CaseManagementData>(dialogContext.Context);
                    if (service != null)
                    {
                        // Push the update case management dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, UpdateCaseManagementDialog.Name, null, cancellationToken);
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync(null, cancellationToken);
                },
                async (dialogContext, cancellationToken) =>
                {
                    // Check if the organization has housing services.
                    var service = await api.GetService<HousingData>(dialogContext.Context);
                    if (service != null)
                    {
                        // Push the update housing dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, UpdateHousingDialog.Name, null, cancellationToken);
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync(null, cancellationToken);
                },
                async (dialogContext, cancellationToken) =>
                {
                    // Check if the organization has job training services.
                    var service = await api.GetService<EmploymentData>(dialogContext.Context);
                    if (service != null)
                    {
                        // Push the update job training dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, UpdateEmploymentDialog.Name, null, cancellationToken);
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync(null, cancellationToken);
                },
                async (dialogContext, cancellationToken) =>
                {
                    // Check if the organization has mental health services.
                    var service = await api.GetService<MentalHealthData>(dialogContext.Context);
                    if (service != null)
                    {
                        // Push the update mental health dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, UpdateMentalHealthDialog.Name, null, cancellationToken);
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync(null, cancellationToken);
                },
                async (dialogContext, cancellationToken) =>
                {
                    // Check if the organization has substance use services.
                    var service = await api.GetService<SubstanceUseData>(dialogContext.Context);
                    if (service != null)
                    {
                        // Push the update substance use dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, UpdateSubstanceUseDialog.Name, null, cancellationToken);
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync(null, cancellationToken);
                },
                async (dialogContext, cancellationToken) =>
                {
                    // End this dialog to pop it off the stack.
                    return await dialogContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
