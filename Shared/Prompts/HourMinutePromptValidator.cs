using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class HourMinutePromptValidator
    {
        public static PromptValidator<string> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                var success = DateTimeHelper.ParseHourAndMinute(promptContext.Context.Activity.Text, out DateTime dt);
                return await Task.FromResult(success);
            };
        }
    }
}
