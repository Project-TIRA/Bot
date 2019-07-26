using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.Models;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialog : DialogBase
    {
        public static string Name = typeof(UpdateHousingDialog).FullName;

        public UpdateHousingDialog(StateAccessors state, DialogSet dialogs, ApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var steps = new List<WaterfallStep>();

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Capacity.Housing.EmergencySharedBeds, nameof(HousingData.EmergencySharedBedsTotal),
                nameof(HousingData.EmergencySharedBedsOpen), nameof(HousingData.HasWaitlist), nameof(HousingData.EmergencySharedBedsWaitlistLength),
                Phrases.Capacity.Housing.GetEmergencySharedBedsOpen));

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Capacity.Housing.EmergencyPrivateBeds, nameof(HousingData.EmergencyPrivateBedsTotal),
                nameof(HousingData.EmergencyPrivateBedsOpen), nameof(HousingData.HasWaitlist), nameof(HousingData.EmergencyPrivateBedsWaitlistLength),
                Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen));

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Capacity.Housing.LongTermSharedBeds, nameof(HousingData.LongTermSharedBedsTotal),
                nameof(HousingData.LongTermSharedBedsOpen), nameof(HousingData.HasWaitlist), nameof(HousingData.LongTermSharedBedsWaitlistLength),
                Phrases.Capacity.Housing.GetLongTermSharedBedsOpen));

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Capacity.Housing.LongTermPrivateBeds, nameof(HousingData.LongTermPrivateBedsTotal),
                nameof(HousingData.LongTermPrivateBedsOpen), nameof(HousingData.HasWaitlist), nameof(HousingData.LongTermPrivateBedsWaitlistLength),
                Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen));

            // End this dialog to pop it off the stack.
            steps.Add(async (stepContext, cancellationToken) => { return await stepContext.EndDialogAsync(cancellationToken); });

            return new WaterfallDialog(Name, steps);
        }
    }
}
