using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Shared;
using System.Threading;
using System.Threading.Tasks;

namespace SearchBot.Bot.Luis
{
    public class LuisHelper : IRecognizer
    {
        private readonly LuisRecognizer recognizer;

        public LuisHelper(IConfiguration configuration)
        {
            bool isConfigured = !string.IsNullOrEmpty(configuration.LuisAppId()) &&
                !string.IsNullOrEmpty(configuration.LuisSubscriptionKey()) &&
                !string.IsNullOrEmpty(configuration.LuisEndpointUrl());

            if (isConfigured)
            {
                this.recognizer = new LuisRecognizer(
                    new LuisApplication(
                        configuration.LuisAppId(),
                        configuration.LuisSubscriptionKey(),
                        configuration.LuisEndpointUrl()));
            }
        }

        public virtual bool IsConfigured => this.recognizer != null;

        public virtual async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
            => await this.recognizer.RecognizeAsync(turnContext, cancellationToken);

        public virtual async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
            where T : IRecognizerConvert, new()
            => await this.recognizer.RecognizeAsync<T>(turnContext, cancellationToken);
    }
}