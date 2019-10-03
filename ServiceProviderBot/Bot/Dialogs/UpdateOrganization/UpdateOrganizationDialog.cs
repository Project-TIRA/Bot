using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization
{
    public class UpdateOrganizationDialog : DialogBase
    {
        public static string Name = typeof(UpdateOrganizationDialog).FullName;

        public UpdateOrganizationDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (dialogContext, cancellationToken) =>
                {
                    var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                    var services = await this.api.GetServices(userContext.OrganizationId);
                    if (services.Count == 0)
                    {
                        // Nothing to update.
                        await Messages.SendAsync(Phrases.Update.NothingToUpdate, dialogContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(cancellationToken);
                    }

                    if (services.Count > 1)
                    {
                        // Give an option to update a specific service or all services.
                        var choices = new List<Choice>();
                        choices.Add(new Choice { Value = Phrases.Services.All });
                        services.ForEach(s => choices.Add(new Choice { Value = Helpers.GetServiceName(s.Type) }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions() {
                                Prompt = Phrases.Update.Options,
                                Choices = choices
                            },
                            cancellationToken);
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync(null, cancellationToken);
                },
                async (dialogContext, cancellationToken) =>
                {
                    if (dialogContext.Result != null && dialogContext.Result is FoundChoice)
                    {
                        // Push the specific dialog onto the stack if one was selected.
                        switch (((FoundChoice)dialogContext.Result).Value)
                        {
                            case Phrases.Services.CaseManagement.ServiceName: return await BeginDialogAsync(dialogContext, UpdateCaseManagementDialog.Name, null, cancellationToken);
                            case Phrases.Services.Housing.ServiceName: return await BeginDialogAsync(dialogContext, UpdateHousingDialog.Name, null, cancellationToken);
                            case Phrases.Services.Employment.ServiceName: return await BeginDialogAsync(dialogContext, UpdateEmploymentDialog.Name, null, cancellationToken);
                            case Phrases.Services.MentalHealth.ServiceName: return await BeginDialogAsync(dialogContext, UpdateMentalHealthDialog.Name, null, cancellationToken);
                            case Phrases.Services.SubstanceUse.ServiceName: return await BeginDialogAsync(dialogContext, UpdateSubstanceUseDialog.Name, null, cancellationToken);
                        }
                    }

                    // Push the update capacity dialog onto the stack.
                    return await BeginDialogAsync(dialogContext, UpdateCapacityDialog.Name, null, cancellationToken);

                },
                async (dialogContext, cancellationToken) =>
                {
                    // Send the closing message.
                    await Messages.SendAsync(Phrases.Update.Closing, dialogContext.Context, cancellationToken);

                    // End this dialog to pop it off the stack.
                    return await dialogContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
