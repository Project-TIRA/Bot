using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ServiceProviderBot.Bot.Dialogs.Capacity
{
    public class UpdateCapacityDialog : DialogBase
    {
        public static string Name = typeof(UpdateCapacityDialog).FullName;

        public UpdateCapacityDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            var waterfallSteps = new List<WaterfallStep>();

            waterfallSteps.Add(GenerateCheckServicesStep());
            waterfallSteps.Add(GenerateRecordServicesStep());

            // Generate update steps for each potential service type.
            // The steps for each type will check if that type should be updated.
            var types = Helpers.GetSubtypes<ServiceData>();

            foreach (var type in types)
            {
                waterfallSteps.Add(GenerateCreateDataStep(type));
                waterfallSteps.AddRange(GenerateUpdateSteps(type));
                waterfallSteps.Add(GenerateCompleteDataStep(type));
            }

            waterfallSteps.Add(GenerateClosingStep());

            return new WaterfallDialog(Name, waterfallSteps);
        }

        private WaterfallStep GenerateCheckServicesStep()
        {
            return async (dialogContext, cancellationToken) =>
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
                else if (services.Count > 1)
                {
                    var typeNames = Helpers.GetServiceTypeNames(services.Select(s => s.Type));

                    // Give an option to update a specific service or all services.
                    var choices = new List<Choice>();
                    choices.Add(new Choice { Value = Phrases.Services.All });
                    typeNames.ForEach(s => choices.Add(new Choice { Value = s }));

                    return await dialogContext.PromptAsync(
                        Prompt.ChoicePrompt,
                        new PromptOptions()
                        {
                            Prompt = Phrases.Update.Options,
                            Choices = choices
                        },
                        cancellationToken);
                }

                // Skip this step.
                return await dialogContext.NextAsync(null, cancellationToken);
            };
        }

        private WaterfallStep GenerateRecordServicesStep()
        {
            return async (dialogContext, cancellationToken) =>
            {
                var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                bool singleService = false;

                if (dialogContext.Result != null && dialogContext.Result is FoundChoice)
                {
                    // Update the specific service type.
                    var result = ((FoundChoice)dialogContext.Result).Value;
                    if (result != Phrases.Services.All)
                    {
                        singleService = true;
                        userContext.TypesToUpdate.Add(Helpers.GetServiceTypeByName(result).ServiceType());
                    }
                }

                if (!singleService)
                {
                    // Get the types of services available.
                    var services = await this.api.GetServices(userContext.OrganizationId);

                    // If the previous result is null then there is only one service type, since there was no prompt to check which to update.
                    Debug.Assert(dialogContext.Result != null || services.Count == 1);

                    // Get the types so that they are alphabetical by type name.
                    var types = Helpers.GetServicesByType(services.Select(s => s.Type));
                    userContext.TypesToUpdate.AddRange(types.Select(t => t.ServiceType()));
                }

                return await dialogContext.NextAsync(null, cancellationToken);
            };
        }

        private List<WaterfallStep> GenerateUpdateSteps(ServiceData type)
        {
            var waterfallSteps = new List<WaterfallStep>();

            foreach (var step in type.UpdateSteps())
            {
                waterfallSteps.Add(async (dialogContext, cancellationToken) =>
                {
                    var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                    if (userContext.TypesToUpdate.Contains(type.ServiceType()))
                    {
                        // Get the service.
                        var service = await this.api.GetService(userContext.OrganizationId, type.ServiceType());

                        // Get the latest snapshot created by the user.
                        var data = await this.api.GetLatestServiceData(userContext.OrganizationId, type, createdByUserTurnContext: dialogContext.Context);
                        var totalPropertyValue = (int)data.GetProperty(step.TotalPropertyName);

                        // Check if the organization has this service.
                        if (totalPropertyValue > 0)
                        {
                            var validations = new LessThanOrEqualPromptValidations()
                            {
                                Max = totalPropertyValue
                            };

                            var prompt = Phrases.Capacity.GetOpenings(step.Name);

                            // Prompt for the open count.
                            return await dialogContext.PromptAsync(
                                Prompt.LessThanOrEqualPrompt,
                                new PromptOptions
                                {
                                    Prompt = prompt,
                                    RetryPrompt = Phrases.Capacity.RetryInvalidCount(totalPropertyValue, prompt),
                                    Validations = validations
                                },
                                cancellationToken);
                        }
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync(null, cancellationToken);
                });

                waterfallSteps.Add(async (dialogContext, cancellationToken) =>
                {
                    // Check if the previous step had a result.
                    if (dialogContext.Result != null)
                    {
                        var open = int.Parse((string)dialogContext.Result);

                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                        // Get the latest snapshot created by the user and update it.
                        var data = await this.api.GetLatestServiceData(userContext.OrganizationId, type, createdByUserTurnContext: dialogContext.Context);
                        data.SetProperty(step.OpenPropertyName, open);
                        await this.api.Update(data);

                        // Check if a waitlist is available.
                        var hasWaitlist = (bool)data.GetProperty(step.HasWaitlistPropertyName);
                        if (hasWaitlist && open == 0)
                        {
                            // Prompt for if the waitlist is open.
                            return await dialogContext.PromptAsync(
                                Prompt.ConfirmPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistIsOpen(step.Name) },
                                cancellationToken);
                        }
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync(null, cancellationToken);
                });

                waterfallSteps.Add(async (dialogContext, cancellationToken) =>
                {
                    // Check if the previous step had a result.
                    if (dialogContext.Result != null)
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                        // Get the latest snapshot created by the user and update it.
                        var data = await this.api.GetLatestServiceData(userContext.OrganizationId, type, createdByUserTurnContext: dialogContext.Context);
                        data.SetProperty(step.WaitlistIsOpenPropertyName, (bool)dialogContext.Result);
                        await this.api.Update(data);
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync(null, cancellationToken);
                });
            }

            return waterfallSteps;
        }

        private WaterfallStep GenerateCreateDataStep(ServiceData type)
        {
            return async (dialogContext, cancellationToken) =>
            {
                var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                if (userContext.TypesToUpdate.Contains(type.ServiceType()))
                {
                    // Get the latest snapshot.
                    var previousData = await this.api.GetLatestServiceData(userContext.OrganizationId, type);

                    // Create a new snapshot and copy the static values from the previous one.
                    var data = Helpers.CreateSubType(type);
                    data.CopyStaticValues(previousData);
                    data.CreatedById = userContext.UserId;
                    await this.api.Create(data);
                }

                // Continue to the next step.
                return await dialogContext.NextAsync(null, cancellationToken);
            };
        }

        private WaterfallStep GenerateCompleteDataStep(ServiceData type)
        {
            return async (dialogContext, cancellationToken) =>
            {
                var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                if (userContext.TypesToUpdate.Contains(type.ServiceType()))
                {
                    // Mark the snapshot created by the user as complete.
                    var data = await this.api.GetLatestServiceData(userContext.OrganizationId, type, createdByUserTurnContext: dialogContext.Context);
                    data.IsComplete = true;
                    await this.api.Update(data);
                }

                // Continue to the next step.
                return await dialogContext.NextAsync(null, cancellationToken);
            };
        }

        private WaterfallStep GenerateClosingStep()
        {
            return async (dialogContext, cancellationToken) =>
            {
                // Send the closing message.
                await Messages.SendAsync(Phrases.Update.Closing, dialogContext.Context, cancellationToken);

                // End this dialog to pop it off the stack.
                return await dialogContext.EndDialogAsync(cancellationToken);
            };
        }
    }
}
