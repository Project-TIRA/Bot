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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Dialogs.Capacity
{
    public class UpdateCapacityDialog : DialogBase
    {
        public static string Name = typeof(UpdateCapacityDialog).FullName;

        public UpdateCapacityDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
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
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        bool singleService = false;

                        if (dialogContext.Result != null && dialogContext.Result is FoundChoice)
                        {
                            var result = ((FoundChoice)dialogContext.Result).Value;
                            if (result != Phrases.Services.All)
                            {
                                // Selected a specific service type to update.
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

                        // Push the services dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, UpdateServicesDialog.Name, null, cancellationToken);
                    }
                });
            });
        }
    }
}
