using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.Utils;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot
{
    public class TheBot : IBot
    {
        private readonly StateAccessors state;
        private readonly DialogSet dialogs;
        private readonly DbInterface database;
        private readonly IConfiguration configuration;

        public TheBot(IConfiguration configuration, StateAccessors state, DbInterface database)
        {
            this.configuration = configuration;

            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            this.database = database ?? throw new ArgumentNullException(nameof(database));

            // Register prompts.
            Utils.Prompts.Register(this.state, this.dialogs, this.database);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            ApiInterface apiInterface = new ApiInterface();
            Shared.Models.Organization org = await apiInterface.GetOrganization("d9e4bba1-84ad-e911-a988-000d3a30dc0a");
            List<Shared.Models.Service> services = await apiInterface.GetServicesForOrganization(org);



            var a = 1;

            //Shared.Models.User user = await apiInterface.GetUser("17be44bf-d9ac-e911-a981-000d3a30d7c8");

            /*
            // Establish context for our dialog from the turn context.
            DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

            // Create the master dialog.
            var masterDialog = new MasterDialog(this.state, this.dialogs, this.database, this.configuration);

            // Attempt to continue any existing conversation.
            DialogTurnResult results = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);
            var startNewConversation = turnContext.Activity.Type == ActivityTypes.Message && results.Status == DialogTurnStatus.Empty;

            // Check if the conversation is expired.
            var forceExpire = Phrases.TriggerReset(turnContext);
            var expired = await this.database.CheckExpiredConversation(turnContext, forceExpire);

            if (expired)
            {
                await dialogContext.CancelAllDialogsAsync(cancellationToken);
                await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
            }
            else if (startNewConversation)
            {
                await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
            }
            */
        }
    }
}
