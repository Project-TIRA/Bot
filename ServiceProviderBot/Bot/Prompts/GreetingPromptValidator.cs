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

                if (string.Equals(message, Phrases.Keywords.Update, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(message, Phrases.Keywords.Enable, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(message, Phrases.Keywords.Disable, StringComparison.OrdinalIgnoreCase))
                {
                    return await Task.FromResult(true);
                }

                return await Task.FromResult(false);
            };
        }
    }
}
