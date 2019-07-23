using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity
{
    public class HousingDialog : DialogBase
    {
        public static string Name = typeof(HousingDialog).FullName;

        public HousingDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for has emergency beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHasHousingEmergency },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Prompt for the total private emergency beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingEmergencyPrivateTotal },
                            cancellationToken);
                    }

                    return await stepContext.NextAsync(null);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Update the profile with the total private emergency beds.
                        var organization = await database.GetOrganization(stepContext.Context);
                        organization.HousingEmergencyPrivateTotal = (int)stepContext.Result;
                        await database.Save();

                        // Prompt for total shared emergency beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingEmergencySharedTotal },
                            cancellationToken);
                    }

                    return await stepContext.NextAsync(null);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Update the profile with the total shared emergency beds.
                        var organization = await database.GetOrganization(stepContext.Context);
                        organization.HousingEmergencySharedTotal = (int)stepContext.Result;
                        await database.Save();
                    }

                    // Prompt for has longterm beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHasHousingLongterm },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Prompt for total private longterm beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingLongtermPrivateTotal },
                            cancellationToken);
                    }

                    return await stepContext.NextAsync(null);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Update the profile with the total private longterm beds.
                        var organization = await database.GetOrganization(stepContext.Context);
                        organization.HousingLongtermPrivateTotal = (int)stepContext.Result;
                        await database.Save();

                        // Prompt for total shared longterm beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingLongtermSharedTotal },
                            cancellationToken);
                    }

                    return await stepContext.NextAsync(null);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Update the profile with the total shared longterm beds.
                        var organization = await database.GetOrganization(stepContext.Context);
                        organization.HousingLongtermSharedTotal = (int) stepContext.Result;
                        await database.Save();
                    }
                    organization.HousingHasWaitlist = (bool)stepContext.Result;
                    await database.Save();

                    // Update housing eligibility
                    return await BeginDialogAsync(stepContext, HousingEligibilityDialog.Name, null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                { 
                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
