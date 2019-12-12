using EntityModel.Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.Dialogs.Search;
using SearchBot.Bot.State;
using SearchBot.Luis;
using Shared;
using Shared.ApiInterface;
using System.Threading;
using System.Threading.Tasks;

namespace SearchBot.Bot.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public MasterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the LUIS result and save any context.
                        var luisResult = await new LuisHelper(this.configuration).RecognizeAsync<LuisModel>(dialogContext.Context, cancellationToken);
                        var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                        await conversationContext.AddLuisResult(this.configuration, luisResult);

                        // Handle the intent.
                        var topIntent = luisResult.TopIntent();
                        if (topIntent.intent == LuisModel.Intent.GetService)
                        {
                            // Push the service dialog onto the stack.
                            return await BeginDialogAsync(dialogContext, SearchDialog.Name, null, cancellationToken);
                        }

                        await Messages.SendAsync(Phrases.Search.GetServiceType, dialogContext.Context, cancellationToken);
                        return await dialogContext.NextAsync(cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Clear the conversation context.
                        await this.state.ClearConversationContext(dialogContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(cancellationToken);
                    }
                });
            });
        }
    }
}
