using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateSubstanceUseDialog : DialogBase
    {
        public static string Name = typeof(UpdateSubstanceUseDialog).FullName;

        public UpdateSubstanceUseDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, string userToken)
            : base(state, dialogs, api, configuration, userToken) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var steps = new List<WaterfallStep>();

            steps.Add(GenerateCreateDataStep<SubstanceUseData>());

            steps.AddRange(GenerateUpdateSteps<SubstanceUseData>(Phrases.Services.SubstanceUse.Detox, nameof(SubstanceUseData.DetoxTotal),
                nameof(SubstanceUseData.DetoxOpen), nameof(SubstanceUseData.HasWaitlist), nameof(SubstanceUseData.DetoxWaitlistLength)));

            steps.AddRange(GenerateUpdateSteps<SubstanceUseData>(Phrases.Services.SubstanceUse.InPatient, nameof(SubstanceUseData.InPatientTotal),
                nameof(SubstanceUseData.InPatientOpen), nameof(SubstanceUseData.HasWaitlist), nameof(SubstanceUseData.InPatientWaitlistLength)));

            steps.AddRange(GenerateUpdateSteps<SubstanceUseData>(Phrases.Services.SubstanceUse.OutPatient, nameof(SubstanceUseData.OutPatientTotal),
                nameof(SubstanceUseData.OutPatientOpen), nameof(SubstanceUseData.HasWaitlist), nameof(SubstanceUseData.OutPatientWaitlistLength)));

            steps.AddRange(GenerateUpdateSteps<SubstanceUseData>(Phrases.Services.SubstanceUse.Group, nameof(SubstanceUseData.GroupTotal),
                nameof(SubstanceUseData.GroupOpen), nameof(SubstanceUseData.HasWaitlist), nameof(SubstanceUseData.GroupWaitlistLength)));

            steps.Add(GenerateCompleteDataStep<SubstanceUseData>());

            // End this dialog to pop it off the stack.
            steps.Add(async (stepContext, cancellationToken) => { return await stepContext.EndDialogAsync(cancellationToken); });

            return new WaterfallDialog(Name, steps);
        }
    }
}
