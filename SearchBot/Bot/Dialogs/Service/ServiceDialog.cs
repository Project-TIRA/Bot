using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.Models;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchBot.Bot.Dialogs.Service
{
    public class ServiceDialog : DialogBase
    {
        public static string Name = typeof(ServiceDialog).FullName;

        public ServiceDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
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
                    // Push the housing dialog onto the stack.
                    return await BeginDialogAsync(dialogContext, HousingDialog.Name, null, cancellationToken);
                },
                async (dialogContext, cancellationToken) =>
                {
                    // Get recommendations based on the context.
                    var recommendations = await GetRecommendations(dialogContext.Context, cancellationToken);
                    await Messages.SendAsync(recommendations, dialogContext.Context, cancellationToken);

                    // End this dialog to pop it off the stack.
                    return await dialogContext.EndDialogAsync(cancellationToken);
                }
            });
        }

        private async Task<string> GetRecommendations(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var conversationContext = await this.state.GetConversationContext(turnContext, cancellationToken);
            Debug.Assert(conversationContext.IsValid());

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

                return $"It looks like {match.Organization.Name} has availability for {conversationContext.ServicesString} services!" +
                    Environment.NewLine + $"You can reach them at {match.Organization.PhoneNumber} or {match.Organization.Address}";
            }

            // Check for multiple organizations that result in a full match.
            var comboMatches = GetMatchCombinations(matchData);
            if (comboMatches.Count >= 1)
            {
                // Either take the only match or the closest matches.
                var combo = comboMatches.Count() == 1 ?
                    comboMatches.First() :
                    comboMatches.Aggregate((c1, c2) => c1.Sum(m => m.Distance) >= c2.Sum(m => m.Distance) ? c1 : c2);

                var result = $"It looks like {combo.Count} organizations can help!";

                foreach (var match in combo)
                {
                    result += Environment.NewLine + Environment.NewLine;
                    result += $"{match.Organization.Name} has availability for {match.ServicesString} services." +
                        Environment.NewLine + $"You can reach them at {match.Organization.PhoneNumber} or {match.Organization.Address}";
                }

                return result;
            }

            // If there are no combination matches, just return the closest organization for each service type.
            return "TODO: No single or combo match";



            // TODO: Allow to search in another location.
            //return $"Unfortunately it looks like no organizations near {conversationContext.Location} have availability for {conversationContext.ServicesString} services";
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
            match.RequestedServiceFlags = conversationContext.ServiceFlags;

            // Filter out organizations that aren't within distance.
            if (!match.IsWithinDistance)
            {
                return match;
            }



            // TODO: Make sure the most recent update was within reasonable time.



            if (conversationContext.HasService(ServiceType.CaseManagement))
            {
                var data = await this.api.GetLatestServiceData<CaseManagementData>(organization.Id);
                if (data != null)
                {
                    match.OrganizationServiceTypes.Add(data.ServiceType);

                    if (data.Open > 0)
                    {
                        match.OrganizationServiceFlags |= ServiceFlags.CaseManagement;
                    }
                }
            }

            if (conversationContext.HasService(ServiceType.Employment))
            {
                var data = await this.api.GetLatestServiceData<EmploymentData>(organization.Id);
                if (data != null)
                {
                    match.OrganizationServiceTypes.Add(data.ServiceType);

                    if (conversationContext.ServiceFlags.HasFlag(ServiceFlags.EmploymentInternship))
                    {
                        if (data.PaidInternshipOpen > 0)
                        {
                            match.OrganizationServiceFlags |= ServiceFlags.EmploymentInternship;
                        }
                    }
                    else
                    {
                        // If not looking for an internship, check if any other types are open.
                        if (data.EmploymentPlacementOpen > 0 ||  data.JobReadinessTrainingOpen > 0 || data.VocationalTrainingOpen > 0)
                        {
                            match.OrganizationServiceFlags |= ServiceFlags.Employment;
                        }
                    }
                }
            }

            if (conversationContext.HasService(ServiceType.Housing))
            {
                var data = await this.api.GetLatestServiceData<HousingData>(organization.Id);
                if (data != null)
                {
                    match.OrganizationServiceTypes.Add(data.ServiceType);

                    if (conversationContext.ServiceFlags.HasFlag(ServiceFlags.HousingEmergency))
                    {
                        if (data.EmergencyPrivateBedsOpen > 0 || data.EmergencySharedBedsOpen > 0)
                        {
                            match.OrganizationServiceFlags |= ServiceFlags.HousingEmergency;
                        }
                    }
                    else if (conversationContext.ServiceFlags.HasFlag(ServiceFlags.HousingLongTerm))
                    {
                        if (data.LongTermPrivateBedsOpen > 0 || data.LongTermSharedBedsOpen > 0)
                        {
                            match.OrganizationServiceFlags |= ServiceFlags.HousingLongTerm;
                        }
                    }
                }
            }

            if (conversationContext.HasService(ServiceType.MentalHealth))
            {
                var data = await this.api.GetLatestServiceData<MentalHealthData>(organization.Id);
                if (data != null)
                {
                    match.OrganizationServiceTypes.Add(data.ServiceType);

                    if (data.InPatientOpen > 0 || data.OutPatientOpen > 0)
                    {
                        match.OrganizationServiceFlags |= ServiceFlags.MentalHealth;
                    }
                }
            }

            if (conversationContext.HasService(ServiceType.SubstanceUse))
            {
                var data = await this.api.GetLatestServiceData<SubstanceUseData>(organization.Id);
                if (data != null)
                {
                    match.OrganizationServiceTypes.Add(data.ServiceType);

                    if (conversationContext.ServiceFlags.HasFlag(ServiceFlags.SubstanceUseDetox))
                    {
                        if (data.DetoxOpen > 0)
                        {
                            match.OrganizationServiceFlags |= ServiceFlags.SubstanceUseDetox;
                        }
                    }
                    else
                    {
                        // If not looking for detox, check if any other types are open.
                        if (data.InPatientOpen > 0 || data.OutPatientOpen > 0 || data.GroupOpen > 0)
                        {
                            match.OrganizationServiceFlags |= ServiceFlags.SubstanceUse;
                        }
                    }
                }
            }

            return match;
        }

        private List<List<MatchData>> GetMatchCombinations(List<MatchData> matchData)
        {
            // Check for combinations of 2 organizations.
            return matchData
                .Join(matchData, Match1 => Match1, Match2 => Match2, (Match1, Match2) => new { Match1, Match2 })
                .Where(pair => (pair.Match1.OrganizationServiceFlags | pair.Match2.OrganizationServiceFlags) == ServiceFlags.All)
                .Select(pair => new List<MatchData>() { pair.Match1, pair.Match2 })
                .ToList();
        }
    }
}
