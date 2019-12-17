using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Dialogs.Preferences
{
    public class DaysDialog : DialogBase
    {
        public static string Name = typeof(DaysDialog).FullName;

        public DaysDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var waterfallSteps = new List<WaterfallStep>();

                // Generate steps for each day of the week.
                foreach (Day day in Enum.GetValues(typeof(Day)))
                {
                    if (day != Day.None)
                    {
                        waterfallSteps.AddRange(GenerateUpdateSteps(day));
                    }
                }

                waterfallSteps.Add(GenerateClosingStep());

                return new WaterfallDialog(Name, waterfallSteps);
            });
        }

        private List<WaterfallStep> GenerateUpdateSteps(Day day)
        {
            return new List<WaterfallStep>()
            {
                async (dialogContext, cancellationToken) =>
                {
                    // Check if they want to be contacted on this day.
                    return await dialogContext.PromptAsync(
                        Prompt.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.Preferences.GetUpdateOnDay(day) },
                        cancellationToken);
                },
                async (dialogContext, cancellationToken) =>
                {
                    // Update the user's preference.
                    var user = await api.GetUser(dialogContext.Context);
                    var contactOnDay = (bool)dialogContext.Result;

                    if (contactOnDay && !user.ReminderFrequency.HasFlag(day))
                    {
                        user.ReminderFrequency |= day;
                        await this.api.Update(user);
                    }
                    else if (!contactOnDay && user.ReminderFrequency.HasFlag(day))
                    {
                        user.ReminderFrequency &= ~day;
                        await this.api.Update(user);
                    }

                    return await dialogContext.NextAsync();
                }
            };
        }

        private WaterfallStep GenerateClosingStep()
        {
            return async (dialogContext, cancellationToken) =>
            {
                await Messages.SendAsync(Phrases.Preferences.Updated, dialogContext.Context, cancellationToken);

                // End this dialog to pop it off the stack.
                return await dialogContext.EndDialogAsync(cancellationToken);
            };
        }
    }
}
