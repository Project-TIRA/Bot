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
            _dialogs = new DialogSet(accessors.ConversationDialogState);

            _masterDialog = new MasterDialog(_accessors, _dialogs);

            _conversationFlow = new ConversationFlow(new List<DialogBase>
            {
                new NewOrgDialog(_accessors, _dialogs),
                new UpdateOrgDialog(_accessors, _dialogs)
            });
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            // Handle the activity type.
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
                        // If there is no active dialog, we should clear the user info and start a new dialog.
                        await _accessors.UserProfile.SetAsync(turnContext, new UserProfile(), cancellationToken);
                        await dialogContext.BeginDialogAsync(_masterDialog.Name, _conversationFlow, cancellationToken);
                        break;

                    case DialogTurnStatus.Complete:

                        await SendMessageAsync("Finished!", turnContext, cancellationToken);
                        break;

                    case DialogTurnStatus.Waiting:
                        // If there is an active dialog, we don't need to do anything here.
                        break;
                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }

            // Save the dialog state into the conversation state.
            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);

            // Save the user profile updates into the user state.
            await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
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

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await stepContext.PromptAsync("name", new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await _accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            // Update the profile.
            userProfile.Name = (string)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Result}."), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Would you like to give your age?") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> AgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                // User said "yes" so we will be prompting for the age.

                // Get the current profile object from user state.
                var userProfile = await _accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

                // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
                return await stepContext.PromptAsync("age", new PromptOptions { Prompt = MessageFactory.Text("Please enter your age.") }, cancellationToken);
            }
            else
            {
                // User said "no" so we will skip the next step. Give -1 as the age.
                return await stepContext.NextAsync(-1, cancellationToken);
            }
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await _accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            // Update the profile.
            userProfile.Age = (int)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            if (userProfile.Age == -1)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"No age given."), cancellationToken);
            }
            else
            {
                // We can send messages to the user at any point in the WaterfallStep.
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I have your age as {userProfile.Age}."), cancellationToken);
            }

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
            return await stepContext.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Is this ok?") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                // Get the current profile object from user state.
                var userProfile = await _accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

                // We can send messages to the user at any point in the WaterfallStep.
                if (userProfile.Age == -1)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I have your name as {userProfile.Name}."), cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I have your name as {userProfile.Name} and age as {userProfile.Age}."), cancellationToken);
                }
            }
            else
            {
                // We can send messages to the user at any point in the WaterfallStep.
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thanks. Your profile will not be kept."), cancellationToken);
            }

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
