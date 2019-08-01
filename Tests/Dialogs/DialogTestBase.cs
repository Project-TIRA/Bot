using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.Utils;
using Shared.ApiInterface;
using Xunit;

namespace Tests.Dialogs
{
    public abstract class DialogTestBase
    {
        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly EfInterface api;
        protected readonly TestAdapter adapter;
        private readonly IConfiguration configuration;

        protected ITurnContext turnContext;
        protected CancellationToken cancellationToken;

        protected DialogTestBase()
        {
            this.state = StateAccessors.Create();
            this.dialogs = new DialogSet(state.DialogContextAccessor);
            this.api = new EfInterface(DbModelFactory.CreateInMemory());
            this.adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(state.ConversationState));

            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true).Build();

            // Register prompts.
            Prompts.Register(this.dialogs);
        }

        protected TestFlow CreateTestFlow(string dialogName, List<ModelBase> initialModels = null)
        {
            return new TestFlow(this.adapter, async (turnContext, cancellationToken) =>
            {
                this.turnContext = turnContext;
                this.cancellationToken = cancellationToken;

                // Initialize the dialog context.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                // Create the master dialog.
                var masterDialog = new MasterDialog(this.state, this.dialogs, this.api, this.configuration);

                // Attempt to continue any existing conversation.
                DialogTurnResult results = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);
                var startNewConversation = turnContext.Activity.Type == ActivityTypes.Message && results.Status == DialogTurnStatus.Empty;

                if (startNewConversation)
                {
                    await InitDatabase(initialModels);
                }

                /*
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
                    // Difference for tests here is starting the given dialog instead of master so that individual dialog flows can be tested.
                    await masterDialog.BeginDialogAsync(dialogContext, dialogName, null, cancellationToken);
                }
                */

                await masterDialog.BeginDialogAsync(dialogContext, dialogName, null, cancellationToken);
            });
        }

        protected Action<IActivity> StartsWith(IMessageActivity expected)
        {
            return receivedActivity =>
            {
                // Validate the received activity.
                var received = receivedActivity.AsMessageActivity();
                Assert.NotNull(received);
                Assert.StartsWith(expected.Text, received.Text);
            };
        }

        protected User CreateTestUser(string organizationId)
        {
            return new User()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test User",
                PhoneNumber = string.Empty,
                OrganizationId = organizationId
            };
        }

        protected Organization CreateTestOrganization(bool isVerified)
        {
            return new Organization()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Organization",
                IsVerified = isVerified,
            };
        }

        private async Task InitDatabase(List<ModelBase> models)
        {
            if (models != null)
            {
                foreach (var model in models)
                {
                    if (model is User)
                    {
                        // Turn context can only be accessed on a turn, so 
                        // this must be called when the bot is executing a turn.
                        ((User)model).Id = this.turnContext.Activity.From.Id;
                    }

                    await this.api.Create(model);
                }
            }
        }
    }
}
