using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Prompts;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Shared.ApiInterface;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization
{
    public class UpdateOrganizationDialog : DialogBase
    {
        public static string Name = typeof(UpdateOrganizationDialog).FullName;

        public UpdateOrganizationDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, string userToken)
            : base(state, dialogs, api, configuration, userToken) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var services = await this.api.GetServices(this.userToken);

                    if (services.Count == 0)
                    {
                        // Nothing to update.
                        await Messages.SendAsync(Phrases.Update.NothingToUpdate, stepContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    if (services.Count > 1)
                    {
                        // Give an option to update a specific service or all services.
                        var choices = new List<Choice>();
                        choices.Add(new Choice { Value = Phrases.Services.All });
                        services.ForEach(s => choices.Add(new Choice { Value = Helpers.GetServiceName(s.Type) }));

                        return await stepContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions() {
                                Prompt = Phrases.Update.Options,
                                Choices = choices
                            },
                            cancellationToken);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Push the specific dialog onto the stack if one was selected.
                        switch (((FoundChoice)stepContext.Result).Value)
                        {
                            case Phrases.Services.CaseManagement.ServiceName: return await BeginDialogAsync(stepContext, UpdateCaseManagementDialog.Name, null, cancellationToken);
                            case Phrases.Services.Housing.ServiceName: return await BeginDialogAsync(stepContext, UpdateHousingDialog.Name, null, cancellationToken);
                            case Phrases.Services.JobTraining.ServiceName: return await BeginDialogAsync(stepContext, UpdateJobTrainingDialog.Name, null, cancellationToken);
                            case Phrases.Services.MentalHealth.ServiceName: return await BeginDialogAsync(stepContext, UpdateMentalHealthDialog.Name, null, cancellationToken);
                            case Phrases.Services.SubstanceUse.ServiceName: return await BeginDialogAsync(stepContext, UpdateSubstanceUseDialog.Name, null, cancellationToken);
                        }
                    }

                    // Push the update capacity dialog onto the stack.
                    return await BeginDialogAsync(stepContext, UpdateCapacityDialog.Name, null, cancellationToken);

                },
                async (stepContext, cancellationToken) =>
                {
                    // Send the closing message.
                    await Messages.SendAsync(Phrases.Update.Closing, stepContext.Context, cancellationToken);

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
