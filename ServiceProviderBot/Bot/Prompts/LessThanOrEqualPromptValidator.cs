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
                var maxCount = promptContext.Options.Validations;

                if (!(maxCount is int))
                {
                    return await Task.FromResult(false);
                }

                if (!int.TryParse(promptContext.Recognized.Value, out var inputValue))
                {
                    return await Task.FromResult(false);
                }

                var success = inputValue <= (int)maxCount;
                return await Task.FromResult(success);
            };
        }
    }
}
