using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Prompts
{
    public static class LessThanOrEqualPromptValidator
    {
        public static PromptValidator<string> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                var maxCount = (long)promptContext.Options.Validations;

                if (!int.TryParse(promptContext.Recognized.Value, out var inputValue))
                {
                    return await Task.FromResult(false);
                }

                var success = inputValue <= maxCount;
                return await Task.FromResult(success);
            };
        }
    }
}
