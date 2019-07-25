﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.Models;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateJobTrainingDialog : DialogBase
    {
        public static string Name = typeof(UpdateJobTrainingDialog).FullName;

        public UpdateJobTrainingDialog(StateAccessors state, DialogSet dialogs, ApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var steps = new List<WaterfallStep>();

            steps.AddRange(GenerateUpdateSteps<JobTrainingData>(Phrases.Capacity.JobTraining.Service, nameof(JobTrainingData.Total),
                nameof(JobTrainingData.Open), nameof(JobTrainingData.HasWaitlist), nameof(JobTrainingData.WaitlistLength),
                Phrases.Capacity.JobTraining.GetServiceOpen));

            // End this dialog to pop it off the stack.
            steps.Add(async (stepContext, cancellationToken) => { return await stepContext.EndDialogAsync(cancellationToken); });

            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, steps);
        }
    }
}
