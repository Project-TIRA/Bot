using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Dialogs;

namespace TestBot.Bot
{
    public class ConversationFlow
    {
        public List<DialogBase> Steps { get; private set; }

        public ConversationFlow(List<DialogBase> steps)
        {
            this.Steps = steps;
        }

        /// <summary>
        /// Increments the conversation to the next dialog
        /// </summary>
        /// <returns>The new dialog name</returns>
        public async Task<string> Step(Accessors accessors, ITurnContext context, CancellationToken cancellationToken)
        {
            // Get the current index.
            var currentIndex = await accessors.ConversationFlowIndex.GetAsync(context, () => 0, cancellationToken);

            if (currentIndex >= this.Steps.Count - 1)
            {
                // Reached the end.
                return string.Empty;
            }
            else
            {
                // Increment the index and set it before returning the next step.
                await accessors.ConversationFlowIndex.SetAsync(context, ++currentIndex, cancellationToken);
                return this.Steps[currentIndex].Name;
            }
        }

        /// <summary>
        /// Adds a new dialog to the conversation flow
        /// </summary>
        public void AddStep(DialogBase step)
        {
            this.Steps.Add(step);
        }
    }
}
