using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.MentalHealth
{
    public class MentalHealthDialog : DialogBase
    {
        public static string Name = typeof(MentalHealthDialog).FullName;

        public MentalHealthDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for working with men
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.MentalHealth.GetHasDemographicMen },
                        cancellationToken); 
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the male demographic
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateMentalHealthGender(Gender.Male, (bool)stepContext.Result);
                    await database.Save();

                    // Prompt for working with women
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.MentalHealth.GetHasDemographicWomen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the female demographic
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateMentalHealthGender(Gender.Female, (bool)stepContext.Result);
                    await database.Save();

                    // Prompt for the age range
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = Phrases.MentalHealth.GetHasDemographicAgeRange
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Push the mental health age range dialog onto the stack
                        return await BeginDialogAsync(stepContext, MentalHealthAgeRangeDialog.Name, null, cancellationToken);
                    }

                    // Update the organization with the default age range
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.SetMentalHealthDefaultAgeRange();
                    await database.Save();

                    // Skip this step
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async(stepContext, cancellationToken) =>
                {
                    // Prompt for in patient total
                    return await stepContext.PromptAsync(Utils.Prompts.IntPrompt, new PromptOptions
                    {
                        Prompt = Phrases.MentalHealth.GetInPatientTotal
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the in patient total
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.MentalHealth_InPatientTotal  = (int) stepContext.Result;
                    await database.Save();

                    // Prompt for out patient total
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.MentalHealth.GetOutPatientTotal },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the out patient total
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.MentalHealth_OutPatientTotal  = (int) stepContext.Result;
                    await database.Save();

                    // Prompt for the wait list
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.MentalHealth.GetHasWaitlist },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the wait list
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.MentalHealth_HasWaitlist  = (bool) stepContext.Result;
                    await database.Save();

                    // Skip this step
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                
                async (stepContext, cancellationToken) =>
                {
                    // End this dialog to pop it off the stack
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
