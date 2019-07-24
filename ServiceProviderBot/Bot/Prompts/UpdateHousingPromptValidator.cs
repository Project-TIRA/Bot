using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using Shared;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Prompts
{
    public class UpdateHousingPromptValidator
    {
        public static PromptValidator<int> Create(string bedType, DbInterface database)
        {
            return async (promptContext, cancellationToken) =>
            {
                var organization = await database.GetOrganization(promptContext.Context);

                int bedTotal = GetBedTotal(bedType, organization);
                int open = promptContext.Recognized.Value;
                if (open > bedTotal)
                {
                    // Send error message.
                    var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(bedTotal));
                    await Messages.SendAsync(error, promptContext.Context, cancellationToken);

                    return await Task.FromResult(false);
                }
                return await Task.FromResult(true);
            };
        }

        private static int GetBedTotal(string bedType, Organization organization)
        {
            if (bedType.Equals(nameof(Organization.HousingEmergencyPrivateTotal))) { return organization.HousingEmergencyPrivateTotal; }
            else if (bedType.Equals(nameof(Organization.HousingEmergencySharedTotal))) { return organization.HousingEmergencySharedTotal; }
            else if (bedType.Equals(nameof(Organization.HousingLongtermPrivateTotal))) { return organization.HousingLongtermPrivateTotal; }
            return organization.HousingLongtermSharedTotal;
        }
    }
}
