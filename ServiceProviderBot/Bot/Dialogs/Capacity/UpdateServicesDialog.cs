using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Dialogs.Capacity
{
    public class UpdateServicesDialog : DialogBase
    {
        public static string Name = typeof(UpdateServicesDialog).FullName;

        public UpdateServicesDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override async Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var userContext = await this.state.GetUserContext(turnContext, cancellationToken);
            var types = Helpers.GetServiceDataTypeByServiceType(userContext.TypesToUpdate);

            var waterfallSteps = new List<WaterfallStep>();

            // Generate update steps for each of the types.
            foreach (var type in types)
            {
                waterfallSteps.Add(GenerateCreateDataStep(type));
                waterfallSteps.AddRange(GenerateUpdateSteps(type));
                waterfallSteps.Add(GenerateCompleteDataStep(type));
            }

            waterfallSteps.Add(GenerateClosingStep());

            return new WaterfallDialog(Name, waterfallSteps);
        }

        private List<WaterfallStep> GenerateUpdateSteps(ServiceData dataType)
        {
            var waterfallSteps = new List<WaterfallStep>();

            foreach (var serviceCategory in dataType.ServiceCategories())
            {
                foreach (var subService in serviceCategory.Services)
                {
                    waterfallSteps.Add(async (dialogContext, cancellationToken) =>
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        if (userContext.TypesToUpdate.Contains(dataType.ServiceType()))
                        {
                            // Get the service.
                            var service = await this.api.GetService(userContext.OrganizationId, dataType.ServiceType());

                            // Get the latest snapshot created by the user.
                            var data = await this.api.GetLatestServiceData(userContext.OrganizationId, dataType, createdByUserTurnContext: dialogContext.Context);
                            var total = (int)data.GetProperty(subService.TotalPropertyName);

                            // Check if the organization has this service.
                            if (total > 0)
                            {
                                var validations = new LessThanOrEqualPromptValidations()
                                {
                                    Max = total
                                };

                                var prompt = Phrases.Capacity.GetOpenings(subService.Name);

                                // Prompt for the open count.
                                return await dialogContext.PromptAsync(
                                        Prompt.LessThanOrEqualPrompt,
                                        new PromptOptions
                                        {
                                            Prompt = prompt,
                                            RetryPrompt = Phrases.Capacity.RetryInvalidCount(total, prompt),
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
                            var data = await this.api.GetLatestServiceData(userContext.OrganizationId, dataType, createdByUserTurnContext: dialogContext.Context);
                            data.SetProperty(subService.OpenPropertyName, open);
                            await this.api.Update(data);

                            // Check if a waitlist is available.
                            var hasWaitlist = (bool)data.GetProperty(subService.HasWaitlistPropertyName);
                            if (hasWaitlist && open == 0)
                            {
                                // Prompt for if the waitlist is open.
                                return await dialogContext.PromptAsync(
                                    Prompt.ConfirmPrompt,
                                    new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistIsOpen(subService.Name) },
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
                            var data = await this.api.GetLatestServiceData(userContext.OrganizationId, dataType, createdByUserTurnContext: dialogContext.Context);
                            data.SetProperty(subService.WaitlistIsOpenPropertyName, (bool)dialogContext.Result);
                            await this.api.Update(data);
                        }

                        // Skip this step.
                        return await dialogContext.NextAsync(null, cancellationToken);
                    });
                }
            }

            return waterfallSteps;
        }

        private WaterfallStep GenerateCreateDataStep(ServiceData dataType)
        {
            return async (dialogContext, cancellationToken) =>
            {
                var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                if (userContext.TypesToUpdate.Contains(dataType.ServiceType()))
                {
                    // Get the latest snapshot.
                    var previousData = await this.api.GetLatestServiceData(userContext.OrganizationId, dataType);

                    // Create a new snapshot and copy the static values from the previous one.
                    var data = Helpers.CreateSubType(dataType);
                    data.CopyStaticValues(previousData);
                    data.CreatedById = userContext.UserId;
                    await this.api.Create(data);
                }

                // Continue to the next step.
                return await dialogContext.NextAsync(null, cancellationToken);
            };
        }

        private WaterfallStep GenerateCompleteDataStep(ServiceData dataType)
        {
            return async (dialogContext, cancellationToken) =>
            {
                var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                if (userContext.TypesToUpdate.Contains(dataType.ServiceType()))
                {
                    // Mark the snapshot created by the user as complete.
                    var data = await this.api.GetLatestServiceData(userContext.OrganizationId, dataType, createdByUserTurnContext: dialogContext.Context);
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
