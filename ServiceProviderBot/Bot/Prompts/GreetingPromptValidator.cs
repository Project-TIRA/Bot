using Microsoft.Bot.Builder.Dialogs;
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
                var organization = await state.GetOrganization(promptContext.Context);
                bool isExistingOrganization = organization != null;

                var message = promptContext.Recognized.Value;

                if ((message == Utils.Phrases.Greeting.New && !isExistingOrganization) ||
                    (message == Utils.Phrases.Greeting.Update && isExistingOrganization))
                {
                    return await Task.FromResult(true);
                }

                return await Task.FromResult(false);
            };
        }
    }
}
