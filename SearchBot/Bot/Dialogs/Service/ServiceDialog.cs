using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;
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
            var organizations = await this.api.GetVerifiedOrganizations();

            // TODO: Filter by location.
            var userLocation = conversationContext.Location;

            /*
            if (conversationContext.HousingEmergency)
            {
                organizations.Where(o => o.)
            }
            */

            // replace this with a valid location filter that will sort organizations based on distance
            organizations = organizations.Where(o => o.Location.ToLower().Contains(userLocation.ToLower())).ToList();

            // If not, recommend multiple for the different needs.

            // Check if there is one organization that meets all criteria.
            organizations = this.GetOrganizationsRecommendation(conversationContext.GetServicesString(), organizations);

            return Shared.Phrases.Services.Response(conversationContext.GetServicesString(), conversationContext.Location, organizations.First().Name).Text;
        }

        /// <summary>
        /// Ranks the organizations on the basis of the amount of services requested they provide
        /// </summary>
        /// <param name="serviceAsked"> A list of the services asked</param>
        /// <param name="orginaztions"> A list of organizations</param>
        /// <returns> A sorted list of organization </returns>
        private List<EntityModel.Organization> GetOrganizationsRecommendation(string serviceAsked, List<EntityModel.Organization> orginaztions)
        {
            List<(EntityModel.Organization, int)> result = new List<(EntityModel.Organization, int)>();
            var serviceRequested = serviceAsked.Split(new string[] { ",", "and" }, System.StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            foreach (var organization in orginaztions)
            {
                int score = 0;
                // find a way to get a string out of the array of services

                var servicesProvided = this.api.GetServicesForOrganization(organization).Result.ToList();

                foreach (var service in serviceRequested)
                {

                    score += servicesProvided.FindAll(x => Helpers.GetServiceName(x.Type) == service).Count();


                }
                result.Add((organization, score));
            }
            return result.OrderByDescending((x => x.Item2)).Select(x => x.Item1).ToList();
        }
    }
}