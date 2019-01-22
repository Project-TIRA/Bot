using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace TestBot.Bot.Prompts
{
    public static class LocationPromptValidator
    {
        public static PromptValidator<string> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                var value = promptContext.Recognized.Value;
                return await Task.FromResult(value.Length == 5);
            };
        }
    }
}
