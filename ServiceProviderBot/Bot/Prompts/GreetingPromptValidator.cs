using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Prompts
{
    public static class GreetingPromptValidator
    {
        public static PromptValidator<string> Create(StateAccessors state)
        {
            return async (promptContext, cancellationToken) =>
            {
                // Check if the organization exists.
                var organization = await state.Database.GetOrganization(promptContext.Context);
                bool isExistingOrganization = organization != null;

                var message = promptContext.Recognized.Value;

                if ((!isExistingOrganization && string.Equals(message, Utils.Phrases.Greeting.New, StringComparison.OrdinalIgnoreCase)) ||
                    isExistingOrganization && string.Equals(message, Utils.Phrases.Greeting.Update, StringComparison.OrdinalIgnoreCase))
                {
                    return await Task.FromResult(true);
                }

                return await Task.FromResult(false);
            };
        }
    }
}
