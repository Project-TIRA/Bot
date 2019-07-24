using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.MentalHealth;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity
{
    public class CapacityDialog : DialogBase
    {
        public static string Name = typeof(CapacityDialog).FullName;

        public CapacityDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        { 
            return new WaterfallDialog(Name, new WaterfallStep[] 
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for housing capacity.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHasHousing },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Push the housing dialog onto the stack.
                        return await BeginDialogAsync(stepContext, HousingDialog.Name, null, cancellationToken);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the mental health.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.MentalHealth.GetHasMentalHealth },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Push the mental health dialog onto the stack.
                        return await BeginDialogAsync(stepContext, MentalHealthDialog.Name, null, cancellationToken);
                    }

                    // Update the profile with the default mental health capacity.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.MentalHealth_HasWaitlist = false;
                    organization.MentalHealth_InPatientTotal = 0;
                    organization.MentalHealth_OutPatientTotal = 0;
                    await database.Save();

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
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
