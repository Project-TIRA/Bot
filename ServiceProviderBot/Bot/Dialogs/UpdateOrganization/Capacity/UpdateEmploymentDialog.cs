using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateEmploymentDialog : DialogBase
    {
        public static string Name = typeof(UpdateEmploymentDialog).FullName;

        public UpdateEmploymentDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var steps = new List<WaterfallStep>();

            steps.Add(GenerateCreateDataStep<EmploymentData>());

            steps.AddRange(GenerateUpdateSteps<EmploymentData>(Phrases.Services.Employment.JobReadinessTraining, nameof(EmploymentData.JobReadinessTrainingTotal),
                nameof(EmploymentData.JobReadinessTrainingOpen), nameof(EmploymentData.JobReadinessTrainingHasWaitlist), nameof(EmploymentData.JobReadinessTrainingWaitlistIsOpen)));

            steps.AddRange(GenerateUpdateSteps<EmploymentData>(Phrases.Services.Employment.PaidInternships, nameof(EmploymentData.PaidInternshipTotal),
                nameof(EmploymentData.PaidInternshipOpen), nameof(EmploymentData.PaidInternshipHasWaitlist), nameof(EmploymentData.PaidInternshipWaitlistIsOpen)));

            steps.AddRange(GenerateUpdateSteps<EmploymentData>(Phrases.Services.Employment.VocationalTraining, nameof(EmploymentData.VocationalTrainingTotal),
                nameof(EmploymentData.VocationalTrainingOpen), nameof(EmploymentData.VocationalTrainingHasWaitlist), nameof(EmploymentData.VocationalTrainingWaitlistIsOpen)));

            steps.AddRange(GenerateUpdateSteps<EmploymentData>(Phrases.Services.Employment.EmploymentPlacement, nameof(EmploymentData.EmploymentPlacementTotal),
                nameof(EmploymentData.EmploymentPlacementOpen), nameof(EmploymentData.EmploymentPlacementHasWaitlist), nameof(EmploymentData.EmploymentPlacementWaitlistIsOpen)));

            steps.Add(GenerateCompleteDataStep<EmploymentData>());

            // End this dialog to pop it off the stack.
            steps.Add(async (dialogContext, cancellationToken) => { return await dialogContext.EndDialogAsync(cancellationToken); });

            return new WaterfallDialog(Name, steps);
        }
    }
}
