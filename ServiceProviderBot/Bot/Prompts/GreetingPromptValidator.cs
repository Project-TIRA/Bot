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

                if (string.Equals(message, Phrases.Greeting.HelpKeyword, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(message, Phrases.Greeting.UpdateKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    return await Task.FromResult(true);
                }

                var validations = (GreetingPromptValidations)promptContext.Options.Validations;

                var valid = (validations.ContactEnabled && string.Equals(message, Phrases.Greeting.DisableKeyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!validations.ContactEnabled && string.Equals(message, Phrases.Greeting.EnableKeyword, StringComparison.OrdinalIgnoreCase));

                return await Task.FromResult(valid);
            };
        }
    }

    public struct GreetingPromptValidations
    {
        public bool ContactEnabled { get; set; }
    }
}
