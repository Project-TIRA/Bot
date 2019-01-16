using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using TestBot.Bot.Dialogs.NewOrg;
using TestBot.Bot.Models;

namespace TestBot.Bot
{
    public class MyBot : IBot
    {
        private readonly Accessors accessors;
        private readonly DialogSet dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/>Used to manage state</param>
        public MyBot(Accessors accessors)
        {
            this.accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
            this.dialogs = new DialogSet(accessors.DialogContext);

            // Initialize global dialogs.
            Utils.Dialogs.Init(accessors, dialogs);
            Utils.Prompts.Init(dialogs);
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
                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);
                DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);

                switch (results.Status)
                {
                    case DialogTurnStatus.Cancelled:
                    case DialogTurnStatus.Empty:
                        {
                            // Start a new conversation if there is none.
                            await dialogContext.BeginDialogAsync(NewOrgDialog.Name, null, cancellationToken);
                            break;
                        }
                    case DialogTurnStatus.Complete:
                        {
                            // Get the current profile object.
                            var profile = await this.accessors.OrganizationProfile.GetAsync(turnContext, () => new OrganizationProfile(), cancellationToken);

                            // Output the profile.
                            await SendMessageAsync(profile.ToString(), turnContext, cancellationToken);

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
                    await SendMessageAsync("Welcome!", turnContext, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Sends a message to the user.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn.</param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        private static async Task SendMessageAsync(string message, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(message), cancellationToken);
        }
    }
}
