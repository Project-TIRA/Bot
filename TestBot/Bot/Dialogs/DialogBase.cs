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

        public static async Task<DialogTurnResult> HandleNextStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationFlow = (ConversationFlow)stepContext.Options;
            conversationFlow.CurrentIndex++;

            if (conversationFlow.CurrentIndex >= conversationFlow.Dialogs.Count)
            {
                return await stepContext.EndDialogAsync(cancellationToken);
            }
            else
            {
                var nextDialog = conversationFlow.Dialogs[conversationFlow.CurrentIndex].Name;
                return await stepContext.ReplaceDialogAsync(nextDialog, conversationFlow, cancellationToken);
            }
        }
    }
}
