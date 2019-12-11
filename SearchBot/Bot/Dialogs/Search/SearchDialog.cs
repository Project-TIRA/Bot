using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
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
                        // Get recommendation based on the context.
                        var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);
                        var recommendation = await GetRecommendation(conversationContext);
                        await Messages.SendAsync(recommendation, dialogContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(cancellationToken);
                    }
                });
            });
        }

        private async Task<Activity> GetRecommendation(ConversationContext conversationContext)
        {
            System.Diagnostics.Debug.Assert(conversationContext.IsValid());

            // Get the verified organizations.
            var verifiedOrganizations = await this.api.GetVerifiedOrganizations();

            // Get the user's location.
            var userLocation = conversationContext.LocationPosition;

            // Score the organizations against the user's request.
            var matchData = new List<MatchData>();

            foreach (var org in verifiedOrganizations)
            {
                // Get the organization's distance from the location.
                var distance = CalcDistance(userLocation.Lat, userLocation.Lon, Convert.ToDouble(org.Latitude), Convert.ToDouble(org.Longitude));

                // Generate the organization match data.
                var match = await GetOrganizationMatchData(org, distance, conversationContext);
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

            // If there are no combination matches, just return the closest organization for each service type.
            //return MessageFactory.Text("TODO: No single or combo match");



            // TODO: Allow to search in another location.
            return Phrases.Search.NoMatch(conversationContext);
        }

        private double CalcDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371;
            double dLat = Math.PI / 180 * (lat2 - lat1);
            double dLon = Math.PI / 180 * (lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos((Math.PI / 180) * lat1) * Math.Cos((Math.PI / 180) * lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R* c;
        }

        private async Task<MatchData> GetOrganizationMatchData(Organization organization, double distance, ConversationContext conversationContext)
        {
            var match = new MatchData();
            match.Organization = organization;
            match.Distance = distance;
            match.RequestedServiceFlags = conversationContext.RequestedServiceFlags;

            // Filter out organizations that aren't within distance.
            if (!match.IsWithinDistance)
            {
                return match;
            }


            // TODO: Make sure the most recent update was within reasonable time.


            // Go through each service requested to check what is available.
            foreach (var type in Helpers.GetServiceDataTypes())
            {
                if (conversationContext.HasService(type))
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
                                break;
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
                .Where(pair => (pair.Match1.OrganizationServiceFlags | pair.Match2.OrganizationServiceFlags).HasFlag(conversationContext.RequestedServiceFlags))
                .Select(pair => new List<MatchData>() { pair.Match1, pair.Match2 })
                .ToList();
        }
    }
}
