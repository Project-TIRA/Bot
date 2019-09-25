using Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.Dialogs.Service;
using SearchBot.Bot.Luis;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;

namespace SearchBot.Bot.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public MasterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (dialogContext, cancellationToken) =>
                {
                    // Get the LUIS result and save any context.
                    var luisResult = await new LuisHelper(this.configuration).RecognizeAsync<LuisModel>(dialogContext.Context, cancellationToken);
                    var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                    conversationContext.AddLuisResult(luisResult);

                    // Handle the intent.
                    var topIntent = luisResult.TopIntent();
                    if (topIntent.intent == LuisModel.Intent.GetService)
                    {
                        // Push the service dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, ServiceDialog.Name, null, cancellationToken);
                    }

                    await Messages.SendAsync(Phrases.Intents.Unknown, dialogContext.Context, cancellationToken);
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
        }
    }
}
