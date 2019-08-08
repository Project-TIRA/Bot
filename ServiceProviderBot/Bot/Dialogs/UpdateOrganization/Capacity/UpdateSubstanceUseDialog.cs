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

        public UpdateSubstanceUseDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var steps = new List<WaterfallStep>();

            steps.Add(GenerateCreateDataStep<SubstanceUseData>());

            steps.AddRange(GenerateUpdateSteps<SubstanceUseData>(Phrases.Capacity.SubstanceUse.DetoxServiceName, nameof(SubstanceUseData.DetoxTotal),
                nameof(SubstanceUseData.DetoxOpen), nameof(SubstanceUseData.HasWaitlist), nameof(SubstanceUseData.DetoxWaitlistLength),
                Phrases.Capacity.SubstanceUse.GetDetoxOpen));

            steps.AddRange(GenerateUpdateSteps<SubstanceUseData>(Phrases.Capacity.SubstanceUse.InPatientServiceName, nameof(SubstanceUseData.InPatientTotal),
                nameof(SubstanceUseData.InPatientOpen), nameof(SubstanceUseData.HasWaitlist), nameof(SubstanceUseData.InPatientWaitlistLength),
                Phrases.Capacity.SubstanceUse.GetInPatientOpen));

            steps.AddRange(GenerateUpdateSteps<SubstanceUseData>(Phrases.Capacity.SubstanceUse.OutPatientServiceName, nameof(SubstanceUseData.OutPatientTotal),
                nameof(SubstanceUseData.OutPatientOpen), nameof(SubstanceUseData.HasWaitlist), nameof(SubstanceUseData.OutPatientWaitlistLength),
                Phrases.Capacity.SubstanceUse.GetOutPatientOpen));

            steps.AddRange(GenerateUpdateSteps<SubstanceUseData>(Phrases.Capacity.SubstanceUse.GroupServiceName, nameof(SubstanceUseData.GroupTotal),
                nameof(SubstanceUseData.GroupOpen), nameof(SubstanceUseData.HasWaitlist), nameof(SubstanceUseData.GroupWaitlistLength),
                Phrases.Capacity.SubstanceUse.GetGroupOpen));

            steps.Add(GenerateCompleteDataStep<SubstanceUseData>());

            // End this dialog to pop it off the stack.
            steps.Add(async (stepContext, cancellationToken) => { return await stepContext.EndDialogAsync(cancellationToken); });

            return new WaterfallDialog(Name, steps);
        }
    }
}
