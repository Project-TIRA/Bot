﻿using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateMentalHealthDialog : DialogBase
    {
        public static string Name = typeof(UpdateMentalHealthDialog).FullName;

        public UpdateMentalHealthDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var steps = new List<WaterfallStep>();

            steps.Add(GenerateCreateDataStep<MentalHealthData>());

            steps.AddRange(GenerateUpdateSteps<MentalHealthData>(Phrases.Capacity.MentalHealth.InPatientName, nameof(MentalHealthData.InPatientTotal),
                nameof(MentalHealthData.InPatientOpen), nameof(MentalHealthData.HasWaitlist), nameof(MentalHealthData.InPatientWaitlistLength),
                Phrases.Capacity.MentalHealth.GetInPatientOpen));

            steps.AddRange(GenerateUpdateSteps<MentalHealthData>(Phrases.Capacity.MentalHealth.OutPatientName, nameof(MentalHealthData.OutPatientTotal),
                nameof(MentalHealthData.OutPatientOpen), nameof(MentalHealthData.HasWaitlist), nameof(MentalHealthData.OutPatientWaitlistLength),
                Phrases.Capacity.MentalHealth.GetOutPatientOpen));

            steps.Add(GenerateCompleteDataStep<MentalHealthData>());

            // End this dialog to pop it off the stack.
            steps.Add(async (stepContext, cancellationToken) => { return await stepContext.EndDialogAsync(cancellationToken); });

            return new WaterfallDialog(Name, steps);
        }
    }
}