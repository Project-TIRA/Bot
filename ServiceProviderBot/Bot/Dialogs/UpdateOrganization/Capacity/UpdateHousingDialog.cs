using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Prompts;
using ServiceProviderBot.Bot.Utils;
using Shared;
using EntityModel;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialog : DialogBase
    {
        public static string Name = typeof(UpdateHousingDialog).FullName;

        public UpdateHousingDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) {

            dialogs.Add(new NumberPrompt<int>(PromptIds.EmergencyPrivateOpen, UpdateHousingPromptValidator.Create(nameof(Organization.HousingEmergencyPrivateTotal), database)));
            dialogs.Add(new NumberPrompt<int>(PromptIds.EmergencySharedOpen, UpdateHousingPromptValidator.Create(nameof(Organization.HousingEmergencySharedTotal), database)));
            dialogs.Add(new NumberPrompt<int>(PromptIds.LongtermPrivateOpen, UpdateHousingPromptValidator.Create(nameof(Organization.HousingLongtermPrivateTotal), database)));
            dialogs.Add(new NumberPrompt<int>(PromptIds.LongtermSharedOpen, UpdateHousingPromptValidator.Create(nameof(Organization.HousingLongtermSharedTotal), database)));
        }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);

                    if (organization.HousingEmergencyPrivateTotal > 0)
                    {
                        // Prompt for the open private emergency beds.
                        return await stepContext.PromptAsync(
                            PromptIds.EmergencyPrivateOpen,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingEmergencyPrivateOpen },
                            cancellationToken);
                    }

                    return await stepContext.NextAsync();
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    if (organization.HousingEmergencyPrivateTotal > 0)
                    {
                        var open = (int) stepContext.Result;

                        // Update the profile with the open beds.
                        var snapshot = await database.GetSnapshot(stepContext.Context);
                        snapshot.BedsEmergencyPrivateOpen = open;
                        await database.Save();

                        // Prompt for the waitlist length if necessary.
                        if (open == 0 && organization.HousingHasWaitlist)
                        {
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetHousingEmergencyPrivateWaitlist },
                                cancellationToken);
                        }
                    }

                    return await stepContext.NextAsync(null);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Update the profile with the emergency private waitlist length.
                        var snapshot = await database.GetSnapshot(stepContext.Context);
                        snapshot.BedsEmergencyPrivateWaitlistLength = (int)stepContext.Result;
                        await database.Save();
                    }

                    var organization = await database.GetOrganization(stepContext.Context);
                    if (organization.HousingEmergencySharedTotal > 0)
                    {
                        // Prompt for the open shared emergency beds.
                        return await stepContext.PromptAsync(
                            PromptIds.EmergencySharedOpen,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingEmergencySharedOpen },
                            cancellationToken);
                    }

                    return await stepContext.NextAsync();
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    if (organization.HousingEmergencySharedTotal > 0)
                    {
                        var open = (int) stepContext.Result;

                        // Update the profile with the open beds.
                        var snapshot = await database.GetSnapshot(stepContext.Context);
                        snapshot.BedsEmergencySharedOpen = open;
                        await database.Save();

                        // Prompt for the waitlist length if necessary.
                        if (open == 0 && organization.HousingHasWaitlist)
                        {
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetHousingEmergencySharedWaitlist },
                                cancellationToken);
                        }
                    }

                    return await stepContext.NextAsync(null);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Update the profile with the emergency shared waitlist length.
                        var snapshot = await database.GetSnapshot(stepContext.Context);
                        snapshot.BedsEmergencySharedWaitlistLength = (int)stepContext.Result;
                        await database.Save();
                    }

                    var organization = await database.GetOrganization(stepContext.Context);
                    if (organization.HousingLongtermPrivateTotal > 0)
                    {
                        // Prompt for the open private longterm beds.
                        return await stepContext.PromptAsync(
                            PromptIds.LongtermPrivateOpen,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingLongtermPrivateOpen },
                            cancellationToken);
                    }

                    return await stepContext.NextAsync();
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    if (organization.HousingLongtermPrivateTotal > 0)
                    {
                        var open = (int) stepContext.Result;

                        // Update the profile with the open beds.
                        var snapshot = await database.GetSnapshot(stepContext.Context);
                        snapshot.BedsLongtermPrivateOpen = open;
                        await database.Save();

                        // Prompt for the waitlist length if necessary.
                        if (open == 0 && organization.HousingHasWaitlist)
                        {
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetHousingLongtermPrivateWaitlist },
                                cancellationToken);
                        }
                    }
                    return await stepContext.NextAsync(null);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Update the profile with the longterm private waitlist length.
                        var snapshot = await database.GetSnapshot(stepContext.Context);
                        snapshot.BedsLongtermPrivateWaitlistLength = (int)stepContext.Result;
                        await database.Save();
                    }
                        // Send error message.
                        var error = Phrases.Capacity.GetHousingError(organization.TotalBeds);
                        await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                    var organization = await database.GetOrganization(stepContext.Context);
                    if (organization.HousingLongtermSharedTotal > 0)
                    {
                        // Prompt for the open shared longterm beds.
                        return await stepContext.PromptAsync(
                            PromptIds.LongtermSharedOpen,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingLongtermSharedOpen },
                            cancellationToken);
                    }

                    return await stepContext.NextAsync();
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    if (organization.HousingLongtermSharedTotal > 0)
                    {
                        var open = (int) stepContext.Result;

                        // Update the profile with the open beds.
                        var snapshot = await database.GetSnapshot(stepContext.Context);
                        snapshot.BedsLongtermSharedOpen = open;
                        await database.Save();

                        // Prompt for the waitlist length if necessary.
                        if (open == 0 && organization.HousingHasWaitlist)
                        {
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetHousingLongtermSharedWaitlist },
                                cancellationToken);
                        }
                    }

                    return await stepContext.NextAsync(null);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Update the profile with the longterm shared waitlist length.
                        var snapshot = await database.GetSnapshot(stepContext.Context);
                        snapshot.BedsLongtermSharedWaitlistLength = (int)stepContext.Result;
                        await database.Save();
                    }
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }

        public static async Task<bool> CanUpdate(StateAccessors state, DbInterface database, ITurnContext context)
        {
            var organization = await database.GetOrganization(context);

            // Updates valid 
            return organization.HousingEmergencyPrivateTotal > 0 || organization.HousingEmergencySharedTotal > 0
                        || organization.HousingLongtermPrivateTotal > 0 || organization.HousingLongtermSharedTotal > 0;
        }

        private class PromptIds
        {
            public const string EmergencyPrivateOpen = "emergencyPrivateOpen";
            public const string EmergencySharedOpen = "emergencySharedOpen";
            public const string LongtermPrivateOpen = "longtermPrivateOpen";
            public const string LongtermSharedOpen = "longtermSharedOpen";
        }

    }
}
