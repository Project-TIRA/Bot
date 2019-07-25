using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCaseManagementDialog : DialogBase
    {
        public static string Name = typeof(UpdateCaseManagementDialog).FullName;

        public UpdateCaseManagementDialog(StateAccessors state, DialogSet dialogs, ApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest case management snapshot.
                    var caseManagementData = await this.api.GetLatestCaseManagementServiceData(Helpers.UserId(stepContext.Context));

                    // Check if the organization has case management spots.
                    if (caseManagementData.SpotsTotal > 0)
                    {
                        // Prompt for the open spots.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.CaseManagement.GetSpotsOpen,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(caseManagementData.SpotsTotal, Phrases.Capacity.CaseManagement.GetSpotsOpen),
                                Validations = caseManagementData.SpotsTotal },
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
                        var caseManagementData = await this.api.GetLatestCaseManagementServiceData(Helpers.UserId(stepContext.Context));
                        caseManagementData.SpotsOpen = open;
                        await caseManagementData.Update(this.api);

                        if (open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.CaseManagement.ServiceName) },
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
                        var caseManagementData = await this.api.GetLatestCaseManagementServiceData(Helpers.UserId(stepContext.Context));
                        caseManagementData.WaitlistLength = (int)stepContext.Result;
                        await caseManagementData.Update(this.api);
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
