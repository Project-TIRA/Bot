using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateJobTrainingDialog : DialogBase
    {
        public static string Name = typeof(UpdateJobTrainingDialog).FullName;

        public UpdateJobTrainingDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var steps = new List<WaterfallStep>();

            steps.Add(GenerateCreateDataStep<JobTrainingData>());

            steps.AddRange(GenerateUpdateSteps<JobTrainingData>(Phrases.Services.JobTraining.ServiceName, nameof(JobTrainingData.Total),
                nameof(JobTrainingData.Open), nameof(JobTrainingData.HasWaitlist), nameof(JobTrainingData.WaitlistIsOpen)));

            steps.Add(GenerateCompleteDataStep<JobTrainingData>());

            // End this dialog to pop it off the stack.
            steps.Add(async (dialogContext, cancellationToken) => { return await dialogContext.EndDialogAsync(cancellationToken); });

            return new WaterfallDialog(Name, steps);
        }
    }
}
