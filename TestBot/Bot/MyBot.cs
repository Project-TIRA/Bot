using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestBot.Bot.Dialogs;

namespace TestBot.Bot
{
    public class MyBot : IBot
    {
        private readonly Accessors _accessors;
        private readonly DialogSet _dialogs;

        private MasterDialog _masterDialog;
        private ConversationFlow _conversationFlow;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/>Used to manage state</param>
        public MyBot(Accessors accessors)
        {
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
            _dialogs = new DialogSet(accessors.DialogContext);

            _masterDialog = new MasterDialog(_accessors, _dialogs);

            _conversationFlow = new ConversationFlow(new List<DialogBase>
            {
                new NewOrgDialog(_accessors, _dialogs),
                new UpdateOrgDialog(_accessors, _dialogs)
            });
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
                DialogContext dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);

                switch (results.Status)
                {
                    case DialogTurnStatus.Cancelled:
                    case DialogTurnStatus.Empty:
                        {
                            // If there is no active dialog, we should clear the data and start a new dialog.
                            await _accessors.ConversationFlowIndex.SetAsync(turnContext, 0, cancellationToken);
                            await dialogContext.BeginDialogAsync(_masterDialog.Name, _conversationFlow, cancellationToken);
                            break;
                        }
                    case DialogTurnStatus.Complete:
                        {
                            await SendMessageAsync("Finished!", turnContext, cancellationToken);
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

            // Save the dialog state into the conversation state.
            //await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);

            // Save the profile updates into the user state.
            //await _accessors.OrganizationState.SaveChangesAsync(turnContext, false, cancellationToken);
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
            var reply = turnContext.Activity.CreateReply(message);
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
