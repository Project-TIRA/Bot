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
    public class RecommendationDialog : DialogBase
    {
        public static string Name = typeof(RecommendationDialog).FullName;

        public RecommendationDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get a recommendation based on the context.
                        var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                        var recommendation = await GetRecommendation(conversationContext);

                        if (recommendation != null)
                        {
                            // Send the recommendation.
                            await Messages.SendAsync(recommendation, dialogContext.Context, cancellationToken);
                            return await dialogContext.NextAsync(null, cancellationToken);
                        }

                        if (!conversationContext.CanExpandSearchDistance())
                        {
                            // Notify that there were no matches.
                            await Messages.SendAsync(Phrases.Search.NoMatch(conversationContext), dialogContext.Context, cancellationToken);
                            return await dialogContext.NextAsync(null, cancellationToken);
                        }

                        // Ask if they would like to search a wider distance.
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Phrases.Search.NoMatchSearchWider(conversationContext) },
                            cancellationToken);

                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Result != null && (bool)dialogContext.Result)
                        {
                            // Expand the search distance.
                            var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                            conversationContext.ExpandSearchDistance();

                            // Restart the dialog.
                            return await dialogContext.ReplaceDialogAsync(Name, null, cancellationToken);
                        }

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(cancellationToken);
                    },
                });
            });
        }

        private async Task<Activity> GetRecommendation(ConversationContext conversationContext)
        {
            System.Diagnostics.Debug.Assert(conversationContext.IsComplete());

            // Get the verified organizations.
            var verifiedOrganizations = await this.api.GetVerifiedOrganizations();

            // Score the organizations against the user's request.
            var matchData = new List<MatchData>();

            foreach (var organization in verifiedOrganizations)
            {
                // Generate the organization match data.
                var match = await GetOrganizationMatchData(organization, conversationContext);
                matchData.Add(match);
            }

            // Check if any organizations fully matches the user's request.
            var fullMatches = matchData.Where(m => m.IsFullMatch);
            if (fullMatches.Count() >= 1)
            {
                // Either take the only match or the closest match.
                var match = fullMatches.Count() == 1 ?
                    fullMatches.First() :
                    fullMatches.Aggregate((m1, m2) => m1.Distance >= m2.Distance ? m1 : m2);

                return Phrases.Search.MakeRecommendation(new List<MatchData>() { match });
            }

            // Check for multiple organizations that result in a full match.
            var comboMatches = GetMatchCombinations(matchData, conversationContext);
            if (comboMatches.Count >= 1)
            {
                // Either take the only match or the closest matches.
                var matches = comboMatches.Count() == 1 ?
                    comboMatches.First() :
                    comboMatches.Aggregate((c1, c2) => c1.Sum(m => m.Distance) >= c2.Sum(m => m.Distance) ? c1 : c2);

                return Phrases.Search.MakeRecommendation(matches);
            }


            // TODO: If there are no combination matches, mention which services
            // couldn't be met and then return the closest organization for each service type.


            // No match found.
            return null;
        }

        private async Task<MatchData> GetOrganizationMatchData(Organization organization, ConversationContext conversationContext)
        {
            // Get the organization's distance from the location.
            Coordinates searchCoordinates = new Coordinates(conversationContext.LocationPosition.Lat, conversationContext.LocationPosition.Lon);
            Coordinates organizationCoordinates = new Coordinates(Convert.ToDouble(organization.Latitude), Convert.ToDouble(organization.Longitude));
            var distance = searchCoordinates.DistanceTo(organizationCoordinates, UnitOfLength.Miles);

            // Create the match data.
            var match = new MatchData();
            match.Organization = organization;
            match.Distance = distance;
            match.RequestedServiceFlags = conversationContext.RequestedServiceFlags();

            // Filter out organizations that aren't within distance.
            if (match.Distance > conversationContext.SearchDistance)
            {
                return match;
            }



            // TODO: Make sure the most recent update was within reasonable time.



            // Go through each service requested to check what is available.
            foreach (var type in Helpers.GetServiceDataTypes())
            {
                if (conversationContext.HasRequestedDataType(type))
                {
                    var data = await this.api.GetLatestServiceData(organization.Id, type);
                    if (data == null)
                    {
                        continue;
                    }

                    foreach (var serviceCategory in data.ServiceCategories())
                    {
                        foreach (var subService in serviceCategory.Services)
                        {
                            var open = (int)data.GetProperty(subService.OpenPropertyName);
                            if (open > 0)
                            {
                                match.OrganizationServiceFlags |= subService.ServiceFlags;
                            }
                        }
                    }
                }
            }

            return match;
        }

        private List<List<MatchData>> GetMatchCombinations(List<MatchData> matchData, ConversationContext conversationContext)
        {
            // Check for combinations of 2 organizations.
            return matchData
                .SelectMany(m => matchData, (Match1, Match2) => new { Match1, Match2 })
                .Distinct()
                .Where(pair => (pair.Match1.OrganizationServiceFlags | pair.Match2.OrganizationServiceFlags).HasFlag(conversationContext.RequestedServiceFlags()))
                .Select(pair => new List<MatchData>() { pair.Match1, pair.Match2 })
                .ToList();
        }       
    }
}
