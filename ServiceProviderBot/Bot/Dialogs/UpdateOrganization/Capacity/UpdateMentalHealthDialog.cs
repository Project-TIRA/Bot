using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.Models;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateSubstanceUseDialog : DialogBase
    {
        public static string Name = typeof(UpdateSubstanceUseDialog).FullName;

        public UpdateSubstanceUseDialog(StateAccessors state, DialogSet dialogs, ApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest substance use snapshot.
                    var mentalHealthData = await this.api.GetLatestServiceData<MentalHealthData>(Helpers.UserId(stepContext.Context), ServiceType.MentalHealth);

                    // Check if the organization has in-patient services.
                    if (mentalHealthData.InPatientTotal > 0)
                    {
                        // Prompt for the open beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.MentalHealth.GetInPatientOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(mentalHealthData.InPatientTotal, Phrases.Capacity.MentalHealth.GetInPatientOpen),
                                Validations = mentalHealthData.InPatientTotal },
                            cancellationToken);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Check if the previous step had a result.
                    if (stepContext.Result != null)
                    {
                        var open = int.Parse((string)stepContext.Result);

                        // Get the latest housing snapshot and update it.
                        var mentalHealthData = await this.api.GetLatestServiceData<MentalHealthData>(Helpers.UserId(stepContext.Context), ServiceType.Housing);
                        mentalHealthData.InPatientOpen = open;
                        await mentalHealthData.Update(this.api);

                        if (open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.MentalHealth.InPatient) },
                                cancellationToken);
                        }
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Check if the previous step had a result.
                    if (stepContext.Result != null)
                    {
                        // Get the latest housing snapshot and update it.
                        var housingData = await this.api.GetLatestServiceData<MentalHealthData>(Helpers.UserId(stepContext.Context), ServiceType.Housing);
                        housingData.InPatientWaitListLength = (int)stepContext.Result;
                        await housingData.Update(this.api);
                    }

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
