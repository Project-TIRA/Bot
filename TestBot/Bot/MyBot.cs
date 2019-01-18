using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using TestBot.Bot.Dialogs;
using TestBot.Bot.Dialogs.NewOrganization;
using TestBot.Bot.Models;

namespace TestBot.Bot
{
    public class MyBot : IBot
    {
        private readonly StateAccessors state;

        private MasterDialog masterDialog { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBot"/> class.
        /// </summary>
        /// <param name="state">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state</param>
        public MyBot(StateAccessors state)
        {
            this.state = state ?? throw new System.ArgumentNullException(nameof(state));
            this.masterDialog = new MasterDialog(state);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                // Processes ConversationUpdate activities to welcome the user.
                if (turnContext.Activity.MembersAdded != null)
                {
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Establish context for our dialog from the turn context.
                DialogContext dialogContext = await this.masterDialog.CreateContextAsync(turnContext, cancellationToken);
                DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);

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
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Sends a welcome message to the user.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn.</param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await Utils.Messages.SendAsync("Welcome!", turnContext, cancellationToken);
                }
            }
        }
    }
}
