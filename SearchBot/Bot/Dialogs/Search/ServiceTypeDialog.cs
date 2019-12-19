using EntityModel.Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using SearchBot.Luis;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace SearchBot.Bot.Dialogs.Search
{
    public class ServiceTypeDialog : DialogBase
    {
        public static string Name = typeof(ServiceTypeDialog).FullName;

        public ServiceTypeDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Check if any service were mentioned.
                        var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                        if (!conversationContext.HasRequestedServices())
                        {
                            // Prompt for the service type.
                            return await dialogContext.PromptAsync(
                                Prompt.TextPrompt,
                                new PromptOptions {
                                    Prompt = Phrases.Search.GetServiceType
                                },
                                cancellationToken);
                        }

                        // Skip this step.
                        return await dialogContext.NextAsync();
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Result != null)
                        {
                            // Get the LUIS result and save any context.
                            var luisResult = await new LuisHelper(this.configuration).RecognizeAsync<LuisModel>(dialogContext.Context, cancellationToken);
                            var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                            await conversationContext.AddLuisResult(this.configuration, luisResult);

                            // Restart this dialog.
                            return await dialogContext.ReplaceDialogAsync(Name, null, cancellationToken);
                        }

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync();
                    }
                });
            });
        }
    }
}
