using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;

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
                    var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));

                    // Check if the organization has detox spaces.
                    if (substanceUseData.DetoxTotal > 0)
                    {
                        // Prompt for the open detox spaces.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.SubstanceUse.GetDetoxOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(substanceUseData.DetoxTotal, Phrases.Capacity.Housing.GetEmergencySharedBedsOpen),
                                Validations = substanceUseData.DetoxTotal },
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
                        var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));
                        substanceUseData.DetoxOpen = open;
                        await substanceUseData.Update(this.api);

                        if (open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.SubstanceUse.DetoxService) },
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
                        // Get the latest substance use snapshot and update it.
                        var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));
                        substanceUseData.DetoxOpen = (int)stepContext.Result;
                        await substanceUseData.Update(this.api);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest substance use snapshot.
                    var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));

                    // Check if the organization has in-patient spaces.
                    if (substanceUseData.InPatientTotal > 0)
                    {
                        // Prompt for the open spaces.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.SubstanceUse.GetInPatientOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(substanceUseData.InPatientTotal, Phrases.Capacity.SubstanceUse.GetInPatientOpen),
                                Validations = substanceUseData.InPatientTotal },
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

                        // Get the latest substance use snapshot and update it.
                        var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));
                        substanceUseData.InPatientOpen = open;
                        await substanceUseData.Update(this.api);

                        if (open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.SubstanceUse.InPatientService) },
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
                        // Get the latest substance use snapshot and update it.
                        var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));
                        substanceUseData.InPatientWaitlistLength = (int)stepContext.Result;
                        await substanceUseData.Update(this.api);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest substance use snapshot.
                    var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));

                    // Check if the organization has out-patient spaces.
                    if (substanceUseData.OutPatientTotal > 0)
                    {
                        // Prompt for the open spaces.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.SubstanceUse.GetOutPatientOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(substanceUseData.OutPatientTotal, Phrases.Capacity.SubstanceUse.GetOutPatientOpen),
                                Validations = substanceUseData.OutPatientTotal },
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

                        // Get the latest substance use snapshot and update it.
                        var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));
                        substanceUseData.OutPatientOpen = open;
                        await substanceUseData.Update(this.api);

                        if (open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.SubstanceUse.OutPatientService) },
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
                        // Get the latest substance use snapshot and update it.
                        var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));
                        substanceUseData.OutPatientWaitlistLength = (int)stepContext.Result;
                        await substanceUseData.Update(this.api);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest substance use snapshot.
                    var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));

                    // Check if the organization has group spaces.
                    if (substanceUseData.GroupTotal > 0)
                    {
                        // Prompt for the open spaces.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.SubstanceUse.GetGroupOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(substanceUseData.GroupTotal, Phrases.Capacity.SubstanceUse.GetGroupOpen),
                                Validations = substanceUseData.GroupTotal },
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

                        // Get the latest substance use snapshot and update it.
                        var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));
                        substanceUseData.GroupOpen = open;
                        await substanceUseData.Update(this.api);

                        if (open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.SubstanceUse.GroupService) },
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
                        // Get the latest substance use snapshot and update it.
                        var substanceUseData = await this.api.GetLatestSubstanceUseServiceData(Helpers.UserId(stepContext.Context));
                        substanceUseData.GroupWaitlistLength= (int)stepContext.Result;
                        await substanceUseData.Update(this.api);
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
