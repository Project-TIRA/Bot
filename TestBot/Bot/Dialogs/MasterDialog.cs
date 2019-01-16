using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Dialogs
{
    public sealed class MasterDialog : DialogBase
    {
        public override string Name { get { return "Master"; } }

        public MasterDialog(Accessors accessors, DialogSet globalDialogSet) : base(accessors)
        {
            // Only gets when the bot is initialized.
            if (globalDialogSet != null)
            {
                // The steps this dialog will take.
                WaterfallStep[] waterfallSteps =
                {
                    BeginAsync,
                    EndAsync
                };

                // Add each dialog to the global dialog set.
                globalDialogSet.Add(new WaterfallDialog(this.Name, waterfallSteps));
            }
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> BeginAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationFlow = stepContext.Options as ConversationFlow;
            if (conversationFlow == null)
            {
                throw new ArgumentNullException(nameof(conversationFlow));
            }

            if (conversationFlow.Dialogs.Count == 0)
            {
                throw new InvalidOperationException(nameof(conversationFlow));
            }

            // Start the first dialog in the conversation flow.
            return await stepContext.BeginDialogAsync(conversationFlow.Dialogs[0].Name, conversationFlow, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> EndAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Dump some info
            var userProfile = await this.accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"Name: {userProfile.Name}, Age: {userProfile.Age}"), cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken);
        }
    }
}
