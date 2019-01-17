using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace TestBot.Bot.Utils
{
    public static class Messages
    {
        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn.</param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public static async Task SendAsync(string message, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(message), cancellationToken);
        }
    }
}
