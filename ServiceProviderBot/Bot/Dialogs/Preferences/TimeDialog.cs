using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Dialogs.Preferences
{
    public class TimeDialog : DialogBase
    {
        public static string Name = typeof(TimeDialog).FullName;

        public TimeDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get their local time to determine their timezone.
                        return await dialogContext.PromptAsync(
                            Prompt.HourMinutePrompt,
                            new PromptOptions {
                                Prompt = Phrases.Preferences.GetCurrentTime,
                                RetryPrompt = Phrases.Preferences.GetCurrentTimeRetry
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the result, which was already validated by the prompt, and convert to an offset.
                        int timezoneOffset = DateTimeHelpers.ConvertToTimezoneOffset((string)dialogContext.Result, DateTime.UtcNow);

                        // Update the user's timezone offset.
                        var user = await api.GetUser(dialogContext.Context);
                        user.TimezoneOffset = timezoneOffset;
                        await this.api.Update(user);

                        return await dialogContext.NextAsync(cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the time they want to be contacted.
                        return await dialogContext.PromptAsync(
                            Prompt.HourPrompt,
                            new PromptOptions {
                                Prompt = Phrases.Preferences.GetUpdateTime,
                                RetryPrompt = Phrases.Preferences.GetUpdateTimeRetry
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the result. This was already validated by the prompt.
                        DateTimeHelpers.ParseHour((string)dialogContext.Result, out DateTime reminderTime);

                        // Update the user's reminder time.
                        var user = await this.api.GetUser(dialogContext.Context);
                        user.ReminderTime = reminderTime.ToShortTimeString();
                        await this.api.Update(user);

                        // Send a confirmation message.
                        await Messages.SendAsync(Phrases.Preferences.UpdateTimeUpdated(user.ReminderTime), dialogContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(cancellationToken);
                    }
                });
            });
        }
    }
}
