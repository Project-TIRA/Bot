using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCaseManagementDialog : DialogBase
    {
        public static string Name = typeof(UpdateCaseManagementDialog).FullName;

        public UpdateCaseManagementDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, string userToken)
            : base(state, dialogs, api, configuration, userToken) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var steps = new List<WaterfallStep>();

            steps.Add(GenerateCreateDataStep<CaseManagementData>());

            steps.AddRange(GenerateUpdateSteps<CaseManagementData>(Phrases.Services.CaseManagement.ServiceName, nameof(CaseManagementData.Total),
                nameof(CaseManagementData.Open), nameof(CaseManagementData.HasWaitlist), nameof(CaseManagementData.WaitlistIsOpen)));

            steps.Add(GenerateCompleteDataStep<CaseManagementData>());

            // End this dialog to pop it off the stack.
            steps.Add(async (stepContext, cancellationToken) => { return await stepContext.EndDialogAsync(cancellationToken); });

            return new WaterfallDialog(Name, steps);
        }
    }
}
