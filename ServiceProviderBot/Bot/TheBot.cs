using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs;
using Shared.ApiInterface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot
{
    public class TheBot : IBot
    {
        private readonly StateAccessors state;
        private readonly DialogSet dialogs;
        private readonly IApiInterface api;
        private readonly IConfiguration configuration;

        public TheBot(IConfiguration configuration, StateAccessors state, CdsInterface api)
        {
            this.configuration = configuration;

            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            this.api = api ?? throw new ArgumentNullException(nameof(api));

            // Register prompts.
            Utils.Prompts.Register(this.dialogs);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Establish context for our dialog from the turn context.
            DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

            // Create the master dialog.
            var masterDialog = new MasterDialog(this.state, this.dialogs, this.api, this.configuration);

            // Attempt to continue any existing conversation.
            DialogTurnResult results = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);
            var startNewConversation = turnContext.Activity.Type == ActivityTypes.Message && results.Status == DialogTurnStatus.Empty;

            /*
            // Check if the conversation is expired.
            var forceExpire = Phrases.TriggerReset(turnContext);
            var expired = await this.database.CheckExpiredConversation(turnContext, forceExpire);

            if (expired)
            {
                await dialogContext.CancelAllDialogsAsync(cancellationToken);
                await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
            }
            else if (startNewConversation)
            {
                await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
            }
            */

            if (startNewConversation)
            {
                await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
            }
        }
    }
}
