using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Middleware
{
    public class TrimIncomingMessageMiddleware : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Trim the incoming text.
                context.Activity.Text.Trim();
            }

            // Invoke the next middleware.
            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
