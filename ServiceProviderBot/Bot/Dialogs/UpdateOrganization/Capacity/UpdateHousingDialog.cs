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
                nameof(HousingData.EmergencySharedBedsOpen), nameof(HousingData.HasWaitlist), nameof(HousingData.EmergencySharedBedsWaitListLength),
                Phrases.Capacity.Housing.GetEmergencySharedBedsOpen));

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Capacity.Housing.EmergencyPrivateBeds, nameof(HousingData.EmergencyPrivateBedsTotal),
                nameof(HousingData.EmergencyPrivateBedsOpen), nameof(HousingData.HasWaitlist), nameof(HousingData.EmergencyPrivateBedsWaitListLength),
                Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen));

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Capacity.Housing.LongTermSharedBeds, nameof(HousingData.LongTermSharedBedsTotal),
                nameof(HousingData.LongTermSharedBedsOpen), nameof(HousingData.HasWaitlist), nameof(HousingData.LongTermSharedBedsWaitListLength),
                Phrases.Capacity.Housing.GetLongTermSharedBedsOpen));

            steps.AddRange(GenerateUpdateSteps<HousingData>(Phrases.Capacity.Housing.LongTermPrivateBeds, nameof(HousingData.LongTermPrivateBedsTotal),
                nameof(HousingData.LongTermPrivateBedsOpen), nameof(HousingData.HasWaitlist), nameof(HousingData.LongTermPrivateBedsWaitListLength),
                Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen));

            // End this dialog to pop it off the stack.
            steps.Add(async (stepContext, cancellationToken) => { return await stepContext.EndDialogAsync(cancellationToken); });

            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, steps);

            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest housing snapshot.
                    var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));

                    // Check if the organization has emergency shared beds.
                    if (housingData.EmergencySharedBedsTotal > 0)
                    {
                        // Prompt for the open beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.Housing.GetEmergencySharedBedsOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(housingData.EmergencySharedBedsTotal, Phrases.Capacity.Housing.GetEmergencySharedBedsOpen),
                                Validations = housingData.EmergencySharedBedsTotal },
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
                        var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));
                        housingData.EmergencySharedBedsOpen = open;
                        await housingData.Update(this.api);

                        if (housingData.HasWaitlist && open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.Housing.EmergencySharedBeds) },
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
                        var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));
                        housingData.EmergencySharedBedsWaitListLength = (int)stepContext.Result;
                        await housingData.Update(this.api);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest housing snapshot.
                    var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));

                    // Check if the organization has emergency private beds.
                    if (housingData.EmergencySharedBedsTotal > 0)
                    {
                        // Prompt for the open beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(housingData.EmergencyPrivateBedsTotal, Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen),
                                Validations = housingData.EmergencyPrivateBedsTotal },
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
                        var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));
                        housingData.EmergencyPrivateBedsOpen = open;
                        await housingData.Update(this.api);

                        if (housingData.HasWaitlist && open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.Housing.EmergencyPrivateBeds) },
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
                        var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));
                        housingData.EmergencyPrivateBedsWaitListLength = (int)stepContext.Result;
                        await housingData.Update(this.api);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest housing snapshot.
                    var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));

                    // Check if the organization has long-term shared beds.
                    if (housingData.EmergencySharedBedsTotal > 0)
                    {
                        // Prompt for the open beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.Housing.GetLongTermSharedBedsOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(housingData.LongTermSharedBedsTotal, Phrases.Capacity.Housing.GetLongTermSharedBedsOpen),
                                Validations = housingData.LongTermSharedBedsTotal },
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
                        var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));
                        housingData.LongTermSharedBedsOpen = open;
                        await housingData.Update(this.api);

                        if (housingData.HasWaitlist && open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.Housing.LongTermSharedBeds) },
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
                        var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));
                        housingData.LongTermSharedBedsWaitListLength = (int)stepContext.Result;
                        await housingData.Update(this.api);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest housing snapshot.
                    var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));

                    // Check if the organization has long-term private beds.
                    if (housingData.EmergencySharedBedsTotal > 0)
                    {
                        // Prompt for the open beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(housingData.LongTermPrivateBedsTotal, Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen),
                                Validations = housingData.LongTermPrivateBedsTotal },
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
                        var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));
                        housingData.LongTermPrivateBedsOpen = open;
                        await housingData.Update(this.api);

                        if (housingData.HasWaitlist && open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.Housing.LongTermPrivateBeds) },
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
                        var housingData = await this.api.GetLatestServiceData<HousingData>(Helpers.UserId(stepContext.Context));
                        housingData.LongTermPrivateBedsWaitListLength = (int)stepContext.Result;
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
