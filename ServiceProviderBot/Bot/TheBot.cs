using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot
{
    public class TheBot : IBot
    {
        private readonly StateAccessors state;
        private readonly DialogSet dialogs;
        private readonly DbInterface database;
        private readonly IConfiguration configuration;

        public TheBot(IConfiguration configuration, StateAccessors state, DbInterface database)
        {
            this.configuration = configuration;

            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            this.database = database ?? throw new ArgumentNullException(nameof(database));

            // Register prompts.
            Utils.Prompts.Register(this.state, this.dialogs, this.database);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Establish context for our dialog from the turn context.
            DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

            // Create the master dialog.
            var masterDialog = new MasterDialog(this.state, this.dialogs, this.database, this.configuration);

            var forceExpire = ShouldReset(turnContext);
            var expired = await this.database.CheckExpiredConversation(turnContext, forceExpire);

            if (expired)
            {
                // Conversation expired, so start a new one.
                await dialogContext.CancelAllDialogsAsync(cancellationToken);
                await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
            }
            else
            {
                DialogTurnResult results = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);

                if (turnContext.Activity.Type == ActivityTypes.Message && results.Status == DialogTurnStatus.Empty)
                {
                    // Begin a new conversation.
                    await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
                }
            }
        }

        private bool ShouldReset(ITurnContext context)
        {
            return /*!this.configuration.IsProduction() &&*/ string.Equals(context.Activity.Text, "reset", StringComparison.OrdinalIgnoreCase);
        }
    }
}
