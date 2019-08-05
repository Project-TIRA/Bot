using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity
{
    public class HousingEligibilityDialog : DialogBase
    {
        public static string Name = typeof(HousingEligibilityDialog).FullName;

        public HousingEligibilityDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the age range.
                    return await stepContext.PromptAsync(Utils.Prompts.ConfirmPrompt, new PromptOptions
                    {
                        Prompt = Phrases.HousingDemographic.GetHasDemographicAgeRange
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if ((bool)stepContext.Result)
                    {
                        // Push the age range dialog onto the stack.
                        return await BeginDialogAsync(stepContext, HousingAgeRangeDialog.Name, null, cancellationToken);
                    }

                    // Update the organization with the default age range.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.SetDefaultAgeRange();
                    await database.Save();

                    return await stepContext.NextAsync();
                },
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for working with men.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.HousingDemographic.GetHasDemographicMen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the male demographic.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateHousingGender(Gender.Male, (bool)stepContext.Result);
                    await database.Save();

                    // Prompt for working with women.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.HousingDemographic.GetHasDemographicWomen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the female demographic.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateHousingGender(Gender.Female, (bool)stepContext.Result);
                    await database.Save();

                    // Prompt for working with non-binary
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.HousingDemographic.GetHasDemographicNonbinary },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the non-binary demographic.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateHousingGender(Gender.NonBinary, (bool)stepContext.Result);
                    await database.Save();

                    // Prompt for working with pregnant demographic
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.HousingDemographic.GetHasDemographicPregnant },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the pregnant demographic.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateHousingFamilyStatus(FamilyStatus.Pregnant, (bool)stepContext.Result);
                    await database.Save();

                    // Prompt for working with parenting demographic
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.HousingDemographic.GetHasDemographicParenting },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with the parenting demographic.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateHousingFamilyStatus(FamilyStatus.Parenting, (bool)stepContext.Result);
                    await database.Save();

                    // Prompt for accepting service animals
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.HousingDemographic.GetAcceptsServiceAnimals },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with if they accept service animals.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.HousingServiceAnimal = (bool) stepContext.Result;
                    await database.Save();

                    // Prompt for non sober demographic
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.HousingDemographic.GetHasDemographicNotSober },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the organization with non sober demographic.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.HousingSobriety = (bool) stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
