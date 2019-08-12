using Microsoft.Bot.Builder.Dialogs;
using Shared;
using System;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Prompts
{
    public static class GreetingPromptValidator
    {
        public static PromptValidator<string> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                var message = promptContext.Recognized.Value;

                if (string.Equals(message, Phrases.Greeting.UpdateKeyword, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(message, Phrases.Greeting.EnableKeyword, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(message, Phrases.Greeting.DisableKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    return await Task.FromResult(true);
                }

                return await Task.FromResult(false);
            };
        }
    }
}
