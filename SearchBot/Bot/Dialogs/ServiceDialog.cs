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

namespace SearchBot.Bot.Dialogs
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

            var results = organizations.Where(o => o.Location == "");


            // If not, recommend multiple for the different needs.


            return $"Here's an organization that can help with {conversationContext.GetServicesString()} in {conversationContext.Location}!";
        }
    }
}
