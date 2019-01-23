using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using ServiceProviderBot.Bot.Dialogs;
using EntityModel;
using System;

namespace ServiceProviderBot.Bot
{
    public class TheBot : IBot
    {
        private readonly DbModel dbContext;
        private readonly StateAccessors state;
        private readonly DialogSet dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="TheBot"/> class.
        /// </summary>
        /// <param name="dbContext">A class containing <see cref="DbModel"/> used to manage DB access</param>
        /// <param name="state">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state</param>
        public TheBot(DbModel dbContext, StateAccessors state)
        {
            this.dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
            this.state = state ?? throw new System.ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            // Register prompts.
            Utils.Prompts.Register(this.dialogs);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Establish context for our dialog from the turn context.
            DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);
            DialogTurnResult results = await DialogBase.ContinueDialogAsync(this.state, this.dialogs, dialogContext, cancellationToken);

            if (ShouldBeginConversation(turnContext))
            {
                await DialogBase.BeginDialogAsync(this.state, this.dialogs, dialogContext, MasterDialog.Name, null, cancellationToken);
            }
        }

        private bool ShouldBeginConversation(ITurnContext turnContext)
        {
            // Process ConversationUpdate activities to know when to the conversation.
            if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                        if (member.Id != turnContext.Activity.Recipient.Id)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
