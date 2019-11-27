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
using System.Net.Http;
using Newtonsoft.Json;

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
            var organizationScores = new List<OrganizationMatch>();

            foreach (var org in verifiedOrganizations)
            {
                // Get the organization's distance from the location.
                var distance = CalcDistance(userLocation.Lat, userLocation.Lon, Convert.ToDouble(org.Latitude), Convert.ToDouble(org.Longitude));

                // Calculate how much the organization matches the user's request.
                var score = await GetOrganizationScore(org, distance, conversationContext);
                organizationScores.Add(score);
            }

            // Check if any organizations fully matched the user's request.
            var fullMatches = organizationScores.Where(m => m.AllServicesMatch);
            if (fullMatches.Count() >= 1)
            {
                // Either take the only match or the closest match.
                var match = fullMatches.Count() == 1 ?
                    fullMatches.First() :
                    fullMatches.Aggregate((m1, m2) => m1.Distance >= m2.Distance ? m1 : m2);

                return $"It looks like {match.Organization.Name} has availability for {conversationContext.ServicesString} services!" +
                    Environment.NewLine + $"You can reach them at {match.Organization.PhoneNumber} or {match.Organization.Address}";
            }

            // Check if any organizations partially matches the user's request.
            var partialMatches = organizationScores.Where(m => m.SomeServicesMatch);
            if (partialMatches.Count() >= 1)
            {
                return "TODO: partial matches";
            }

            // TODO: Allow to search in another location.
            return $"Unfortunately it looks like no organizations near {conversationContext.Location} have availability for {conversationContext.ServicesString} services";
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

        private async Task<OrganizationMatch> GetOrganizationScore(Organization organization, double distance, ConversationContext conversationContext)
        {
            var match = new OrganizationMatch();
            match.Organization = organization;
            match.Distance = distance;

            // Filter out organizations that aren't within distance.
            if (!match.IsWithinDistance())
            {
                return match;
            }



            // TODO: Make sure the most recent update was within reasonable time.



            if (conversationContext.HasService(ServiceType.CaseManagement))
            {
                // Get the latest service data for the org. Returns null if they don't have the service.
                var data = await this.api.GetLatestServiceData<CaseManagementData>(organization.Id);
                if (data != null)
                {
                    match.ServiceData.Add(data);

                    if (data.Open > 0)
                    {
                        match.ServiceFlags |= ServiceFlags.CaseManagement;
                    }
                }
            }

            if (conversationContext.HasService(ServiceType.Employment))
            {
                // Get the latest service data for the org. Returns null if they don't have the service.
                var data = await this.api.GetLatestServiceData<EmploymentData>(organization.Id);
                if (data != null)
                {
                    match.ServiceData.Add(data);

                    if (conversationContext.ServiceFlags.HasFlag(ServiceFlags.EmploymentInternship))
                    {
                        if (data.PaidInternshipOpen > 0)
                        {
                            match.ServiceFlags |= ServiceFlags.EmploymentInternship;
                        }
                    }
                    else
                    {
                        // If not looking for an internship, check if any other types are open.
                        if (data.EmploymentPlacementOpen > 0 ||  data.JobReadinessTrainingOpen > 0 || data.VocationalTrainingOpen > 0)
                        {
                            match.ServiceFlags |= ServiceFlags.Employment;
                        }
                    }
                }
            }

            if (conversationContext.HasService(ServiceType.Housing))
            {
                // Get the latest service data for the org. Returns null if they don't have the service.
                var data = await this.api.GetLatestServiceData<HousingData>(organization.Id);
                if (data != null)
                {
                    match.ServiceData.Add(data);

                    if (conversationContext.ServiceFlags.HasFlag(ServiceFlags.HousingEmergency))
                    {
                        if (data.EmergencyPrivateBedsOpen > 0 || data.EmergencySharedBedsOpen > 0)
                        {
                            match.ServiceFlags |= ServiceFlags.HousingEmergency;
                        }
                    }
                    else if (conversationContext.ServiceFlags.HasFlag(ServiceFlags.HousingLongTerm))
                    {
                        if (data.LongTermPrivateBedsOpen > 0 || data.LongTermSharedBedsOpen > 0)
                        {
                            match.ServiceFlags |= ServiceFlags.HousingLongTerm;
                        }
                    }
                }
            }

            if (conversationContext.HasService(ServiceType.MentalHealth))
            {
                // Get the latest service data for the org. Returns null if they don't have the service.
                var data = await this.api.GetLatestServiceData<MentalHealthData>(organization.Id);
                if (data != null)
                {
                    match.ServiceData.Add(data);

                    if (data.InPatientOpen > 0 || data.OutPatientOpen > 0)
                    {
                        match.ServiceFlags |= ServiceFlags.MentalHealth;
                    }
                }
            }

            if (conversationContext.HasService(ServiceType.SubstanceUse))
            {
                // Get the latest service data for the org. Returns null if they don't have the service.
                var data = await this.api.GetLatestServiceData<SubstanceUseData>(organization.Id);
                if (data != null)
                {
                    match.ServiceData.Add(data);

                    if (conversationContext.ServiceFlags.HasFlag(ServiceFlags.SubstanceUseDetox))
                    {
                        if (data.DetoxOpen > 0)
                        {
                            match.ServiceFlags |= ServiceFlags.SubstanceUseDetox;
                        }
                    }
                    else
                    {
                        // If not looking for detox, check if any other types are open.
                        if (data.InPatientOpen > 0 || data.OutPatientOpen > 0 || data.GroupOpen > 0)
                        {
                            match.ServiceFlags |= ServiceFlags.SubstanceUse;
                        }
                    }
                }
            }

            return match;
        }
    }
}
