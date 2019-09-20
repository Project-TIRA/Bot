using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using EntityModel;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.CaseManagement
{
    public class CaseManagementDemographDialog : DialogBase
    {
        public static string Name = typeof(CaseManagementDemographDialog).FullName;

        public CaseManagementDemographDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Check if organization works with a specific demographic
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.CaseManagement.GetHasDemographic },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result == false)
                    {
                        // End this dialog to pop it off the stack if there is no
                        // specific demographic constraints
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Prompt for working with men.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.CaseManagement.GetHasDemographicMen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                { 
                    // Update the organization with the male demographic.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateCaseManagementGender(Gender.Male, (bool)stepContext.Result);
                    await database.Save();

                    // Prompt for working with women.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.CaseManagement.GetHasDemographicWomen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the female demographic.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateCaseManagementGender(Gender.Female, (bool)stepContext.Result);
                    await database.Save();

                    // Prompt for the age range.
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = Phrases.CaseManagement.GetHasDemographicAgeRange
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Push the age range dialog onto the stack.
                        return await BeginDialogAsync(stepContext, CaseManagementAgeRangeDialog.Name, null, cancellationToken);
                    }

                    // Update the organization with the default age range.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.SetDefaultAgeRangeCaseManagement();
                    await database.Save();

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);

                },
                async (stepContext, cancellationToken) =>
                {
                     // Prompt for sobriety.
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = Phrases.CaseManagement.GetHasSobriety
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with sobriety requirement.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.CaseManagementSobriety = (bool)stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
