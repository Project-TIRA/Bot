using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Dialogs
{
    public abstract class DialogBase
    {
        protected readonly Accessors accessors;

        /// <summary>
        /// Gets the name
        /// </summary>
        public virtual string Name { get; }

        protected DialogBase(Accessors accessors)
        {
            this.accessors = accessors;
        }

        public async Task<DialogTurnResult> HandleNextConversationStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationFlow = (ConversationFlow)stepContext.Options;
            var nextDialog = await conversationFlow.Step(this.accessors, stepContext.Context, cancellationToken);

            return string.IsNullOrEmpty(nextDialog) ?
                await stepContext.EndDialogAsync(cancellationToken) :
                await stepContext.ReplaceDialogAsync(nextDialog, conversationFlow, cancellationToken);
        }
    }
}
