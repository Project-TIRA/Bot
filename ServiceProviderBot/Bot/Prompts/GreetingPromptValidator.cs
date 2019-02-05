using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Utils;
using Shared;
using System;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Prompts
{
    public static class GreetingPromptValidator
    {
        public static PromptValidator<string> Create(StateAccessors state, DbInterface database)
        {
            return async (promptContext, cancellationToken) =>
            {
                // Check if the organization exists.
                var organization = await database.GetOrganization(promptContext.Context);
                bool isExistingOrganization = organization != null;

                var message = promptContext.Recognized.Value;

                if ((!isExistingOrganization && string.Equals(message, Phrases.Greeting.New, StringComparison.OrdinalIgnoreCase)) ||
                    isExistingOrganization && string.Equals(message, Phrases.Greeting.Update, StringComparison.OrdinalIgnoreCase))
                {
                    return await Task.FromResult(true);
                }

                return await Task.FromResult(false);
            };
        }
    }
}
