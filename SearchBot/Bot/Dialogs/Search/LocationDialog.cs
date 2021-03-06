﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace SearchBot.Bot.Dialogs.Search
{
    public class LocationDialog : DialogBase
    {
        public static string Name = typeof(LocationDialog).FullName;

        public LocationDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Check if the location is known.
                        var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                        if (string.IsNullOrEmpty(conversationContext.Location))
                        {
                            // Prompt for the location.
                            return await dialogContext.PromptAsync(
                                Prompt.LocationTextPrompt,
                                new PromptOptions {
                                    Prompt = Phrases.Search.GetLocation,
                                    RetryPrompt = Phrases.Search.GetLocationRetry
                                },
                                cancellationToken);
                            }

                        // Skip this step.
                        return await dialogContext.NextAsync();
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Result != null)
                        {
                            // Save the location.
                            var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                            await conversationContext.SetLocation(this.configuration, (string)dialogContext.Result);
                        }

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(cancellationToken);
                    }
                });
            });
        }
    }
}
