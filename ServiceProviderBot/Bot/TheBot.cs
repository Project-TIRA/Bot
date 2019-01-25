using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using ServiceProviderBot.Bot.Dialogs;
using Microsoft.Extensions.Configuration;
using System;

namespace ServiceProviderBot.Bot
{
    public class TheBot : IBot
    {
        private readonly IConfiguration configuration;

        private readonly StateAccessors state;
        private readonly DialogSet dialogs;

        public TheBot(IConfiguration configuration, StateAccessors state)
        {
            this.configuration = configuration;

            this.state = state ?? throw new System.ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            // Register prompts.
            Utils.Prompts.Register(this.state, this.dialogs);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Establish context for our dialog from the turn context.
            DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

            var expired = ShouldReset(turnContext) || await this.state.Database.CheckExpiredConversation(turnContext);
            if (expired)
            {
                // Conversation expired, so start a new one.
                await dialogContext.CancelAllDialogsAsync(cancellationToken);
                await Utils.Dialogs.BeginDialogAsync(this.state, this.dialogs, dialogContext, MasterDialog.Name, null, cancellationToken);
            }
            else
            {
                DialogTurnResult results = await Utils.Dialogs.ContinueDialogAsync(this.state, this.dialogs, dialogContext, cancellationToken);

                if (turnContext.Activity.Type == ActivityTypes.Message &&
                    results.Status == DialogTurnStatus.Empty)
                {
                    // Begin a new conversation.
                    await Utils.Dialogs.BeginDialogAsync(this.state, this.dialogs, dialogContext, MasterDialog.Name, null, cancellationToken);
                }
            }
        }

        private bool ShouldReset(ITurnContext context)
        {
            return !IsProduction() && string.Equals(context.Activity.Text, "reset", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsProduction()
        {
            return this.configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Production";
        }
    }
}
