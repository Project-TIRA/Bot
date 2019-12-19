using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchBot.Bot.Dialogs.Search
{
    public class SearchDialog : DialogBase
    {
        public static string Name = typeof(SearchDialog).FullName;

        public SearchDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Push the location dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, LocationDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Push the service type dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, ServiceTypeDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Push the services dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, ServicesDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Push the recommendation dialog onto the stack.
                        return await BeginDialogAsync(dialogContext, RecommendationDialog.Name, null, cancellationToken);
                    },
                });
            });
        }
    }
}
