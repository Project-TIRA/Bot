using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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


            // Check if there is one organization that meets all criteria.


            // TODO: Filter by location.

            /*
            if (conversationContext.HousingEmergency)
            {
                organizations.Where(o => o.)
            }
            */

            string sub_key = "ay41VKwaVczc7rlvS9krCupc_OQybqGBLGFz9IsDZoc";
            string query = conversationContext.Location;

            var http = new HttpClient();
            var url = string.Format("https://atlas.microsoft.com/search/fuzzy/json?api-version=1.0&subscription-key={0}&format=json&query={1}", sub_key, query);
            var response = await http.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            RootObject data = (RootObject)serializer.ReadObject(ms);

            string userLat = Convert.ToString(data.results[0].position.lat);
            string userLon = Convert.ToString(data.results[0].position.lon);

            
            var results = organizations.Where(o => CalcDistance(userLat, userLon, o.Latitude, o.Longitude) < 50.0).ToList();


            // If not, recommend multiple for the different needs.
            string res = "";
            for(int i = 0; i < results.Count; i++)
            {
                res = res + " " + Convert.ToString(results[i].Name);
            }
            return res;
            //return $"Here's an organization that can help with {conversationContext.GetServicesString()} in {conversationContext.Location}!";
        }

        private double CalcDistance(string lat1, string lon1, string lat2, string lon2)
        {

            double R = 6371;
            double dLat = (Math.PI / 180) * (Convert.ToDouble(lat2) - Convert.ToDouble(lat1));
            double dLon = (Math.PI / 180) * (Convert.ToDouble(lon2) - Convert.ToDouble(lon1));
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos((Math.PI / 180) * (Convert.ToDouble(lat1))) * Math.Cos((Math.PI / 180) * (Convert.ToDouble(lat2))) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
