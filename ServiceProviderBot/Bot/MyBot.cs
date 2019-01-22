using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using ServiceProviderBot.Bot.Dialogs;

namespace ServiceProviderBot.Bot
{
    public class MyBot : IBot
    {
        private readonly StateAccessors state;
        private readonly DialogSet dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBot"/> class.
        /// </summary>
        /// <param name="state">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state</param>
        public MyBot(StateAccessors state)
        {
            this.state = state ?? throw new System.ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            // Register dialogs and prompts.
            Utils.Dialogs.Register(this.dialogs, this.state);
            Utils.Prompts.Register(this.dialogs);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Establish context for our dialog from the turn context.
            DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);
            DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);

            if (ShouldBeginConversation(turnContext))
            {
                await dialogContext.BeginDialogAsync(MasterDialog.Name, null, cancellationToken);
            }

            /*
            else if (turnContext.Activity.Type == ActivityTypes.Message)
            {
               
                switch (results.Status)
                {
                    case DialogTurnStatus.Cancelled:
                    case DialogTurnStatus.Empty:
                        {
                            // Start a new conversation if there is none.
                            await dialogContext.BeginDialogAsync(MasterDialog.Name, null, cancellationToken);
                            break;
                        }
                    case DialogTurnStatus.Complete:
                        {
                            // Get the current profile object.
                            var profile = await this.state.GetOrganizationProfile(turnContext, cancellationToken);

                            // Output the profile.
                            await Utils.Messages.SendAsync(profile.ToString(), turnContext, cancellationToken);

                            break;
                        }
                    case DialogTurnStatus.Waiting:
                        {
                            // If there is an active dialog, we don't need to do anything here.
                            break;
                        }
                }
            }
            */
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
