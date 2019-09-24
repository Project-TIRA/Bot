using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using System;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialog : DialogBase
    {
        public static string Name = typeof(UpdateHousingDialog).FullName;

        public UpdateHousingDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var steps = new List<WaterfallStep>();

            steps.Add(GenerateCreateDataStep<HousingData>());

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Services.Housing.EmergencySharedBeds, nameof(HousingData.EmergencySharedBedsTotal),
                nameof(HousingData.EmergencySharedBedsOpen), nameof(HousingData.EmergencySharedBedsHasWaitlist), nameof(HousingData.EmergencySharedBedsWaitlistIsOpen)));

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Services.Housing.EmergencyPrivateBeds, nameof(HousingData.EmergencyPrivateBedsTotal),
                nameof(HousingData.EmergencyPrivateBedsOpen), nameof(HousingData.EmergencyPrivateBedsHasWaitlist), nameof(HousingData.EmergencyPrivateBedsWaitlistIsOpen)));

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Services.Housing.LongTermSharedBeds, nameof(HousingData.LongTermSharedBedsTotal),
                nameof(HousingData.LongTermSharedBedsOpen), nameof(HousingData.LongTermSharedBedsHasWaitlist), nameof(HousingData.LongTermSharedBedsWaitlistIsOpen)));

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Services.Housing.LongTermPrivateBeds, nameof(HousingData.LongTermPrivateBedsTotal),
                nameof(HousingData.LongTermPrivateBedsOpen), nameof(HousingData.LongTermPrivateBedsHasWaitlist), nameof(HousingData.LongTermPrivateBedsWaitlistIsOpen)));

            steps.Add(GenerateCompleteDataStep<HousingData>());

            // End this dialog to pop it off the stack.
            steps.Add(async (dialogContext, cancellationToken) => { return await dialogContext.EndDialogAsync(cancellationToken); });

            return new WaterfallDialog(Name, steps);
        }
    }
}
