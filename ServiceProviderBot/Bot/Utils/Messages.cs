using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace ServiceProviderBot.Bot.Utils
{
    public static class Messages
    {
        public static async Task SendAsync(Activity message, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(message, cancellationToken);
        }
    }
}
