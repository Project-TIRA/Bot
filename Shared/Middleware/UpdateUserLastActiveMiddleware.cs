using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Middleware
{
    public class UpdateUserLastActiveMiddleware : IMiddleware
    {
        IApiInterface api;

        public UpdateUserLastActiveMiddleware(IApiInterface api)
        {
            this.api = api;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Invoke the next middleware.
            await next(cancellationToken).ConfigureAwait(false);

            // Run after the bot has run (as the middleware stack unwinds).
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var user = await this.api.GetUser(turnContext);
                if (user != null)
                {
                    user.LastActiveTime = DateTime.UtcNow;
                    await this.api.Update(user);
                }
            }
        }
    }
}
