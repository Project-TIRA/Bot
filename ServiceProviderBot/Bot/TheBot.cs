using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.Prompts;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Shared.ApiInterface;
using System;
using System.Diagnostics;
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
        private string userToken;

        public TheBot(IConfiguration configuration, StateAccessors state, EfInterface api)
        {
            this.configuration = configuration;

            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            this.api = api ?? throw new ArgumentNullException(nameof(api));

            // Register prompts.
            Prompt.Register(this.dialogs);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.userToken = Helpers.GetUserToken(turnContext);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Establish context for our dialog from the turn context.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                // Create the master dialog.
                var masterDialog = new MasterDialog(this.state, this.dialogs, this.api, this.configuration, this.userToken);

                // Start a new conversation if there isn't one already.
                if (dialogContext.Stack.Count == 0)
                {
                    await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
                }
                else
                {
                    // Check if the conversation is expired.
                    var forceExpire = Phrases.Reset.ShouldReset(this.configuration, turnContext);
                    var expired = forceExpire || await this.api.IsUpdateExpired(this.userToken);

                    if (expired)
                    {
                        await dialogContext.CancelAllDialogsAsync(cancellationToken);

                        var user = await api.GetUser(this.userToken);
                        await Messages.SendAsync(forceExpire ? Phrases.Reset.Forced(user) : Phrases.Reset.Expired(user), turnContext, cancellationToken);
                    }
                    else
                    {
                        // Attempt to continue any existing conversation.
                        DialogTurnResult results = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);
                        Debug.Assert(results.Status != DialogTurnStatus.Empty, "Should have an existing conversation");
                    }
                }
            }
        }
    }
}
