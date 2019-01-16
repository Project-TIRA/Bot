using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Dialogs
{
    public abstract class DialogBase
    {
        protected readonly Accessors accessors;
        protected readonly DialogSet globalDialogSet;

        /// <summary>
        /// Gets the name
        /// </summary>
        public virtual string Name { get; }

        protected DialogBase(Accessors accessors, DialogSet globalDialogSet)
        {
            this.accessors = accessors;
        }

        public async Task<DialogTurnResult> HandleNextConversationStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current index.
            var currentIndex = await accessors.ConversationFlowIndex.GetAsync(stepContext.Context, () => 0, cancellationToken);

            // Get the next dialog.
            var conversationFlow = (ConversationFlow)stepContext.Options;
            var nextDialog = conversationFlow.Step(ref currentIndex);

            // Save the index if stepping updated it.
            await accessors.ConversationFlowIndex.SetAsync(stepContext.Context, currentIndex, cancellationToken);

            return string.IsNullOrEmpty(nextDialog) ?
                await stepContext.EndDialogAsync(cancellationToken) :
                await stepContext.ReplaceDialogAsync(nextDialog, conversationFlow, cancellationToken);
        }
    }
}
