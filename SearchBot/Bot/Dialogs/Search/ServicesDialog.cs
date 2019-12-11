using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchBot.Bot.Dialogs.Search
{
    public class ServicesDialog : DialogBase
    {
        public static string Name = typeof(ServicesDialog).FullName;

        public ServicesDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override async Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var conversationContext = await this.state.GetConversationContext(turnContext, cancellationToken);

            var waterfallSteps = new List<WaterfallStep>();

            // Generate steps for each of the types.
            foreach (var dataType in conversationContext.RequestedServices.Select(s => s.DataType()))
            {
                waterfallSteps.AddRange(GenerateCheckSteps(dataType));
            }

            return new WaterfallDialog(Name, waterfallSteps);
        }

        private List<WaterfallStep> GenerateCheckSteps(ServiceData dataType)
        {
            return new List<WaterfallStep>()
            {
                async (dialogContext, cancellationToken) =>
                {
                    var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);

                    // Check if the service needs clarification.
                    if (!conversationContext.IsServiceValid(dataType))
                    {
                        // Prompt for the specific type.
                        var choices = new List<Choice>();
                        dataType.ServiceCategories().ForEach(c => choices.Add(new Choice { Value = c.Name }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions {
                                Prompt = Phrases.Search.GetSpecificType(dataType),
                                Choices = choices
                            },
                            cancellationToken);
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync();
                },
                async (dialogContext, cancellationToken) =>
                {
                    if (dialogContext.Result != null && dialogContext.Result is FoundChoice)
                    {
                        var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);

                        // Update the conversation context with the specific type.
                        var result = ((FoundChoice)dialogContext.Result).Value;
                        var match = dataType.ServiceCategories().FirstOrDefault(c => c.Name == result);
                        conversationContext.CreateOrUpdateServiceContext(dataType, match.ServiceFlags());
                    }

                    // End this dialog to pop it off the stack.
                    return await dialogContext.EndDialogAsync(cancellationToken);
                }
            };
        }
    }
}
