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
using Shared;
using Shared.ApiInterface;
using Xunit;

namespace Tests.Dialogs
{
    public abstract class DialogTestBase
    {
        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly IApiInterface api;
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

        protected TestFlow CreateTestFlow(string dialogName, User user = null)
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
                    await InitUser(user);
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

        protected async Task<Organization> CreateOrganization(bool isVerified)
        {
            var organization = new Organization()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Organization",
                IsVerified = isVerified
            };

            await this.api.Create(organization);
            return organization;
        }

        protected User CreateUser(string organizationId)
        {
            var user = new User()
            {
                OrganizationId = organizationId,
                Name = "Test User",
                PhoneNumber = "+12223334444",
            };

            return user;
        }

        protected async Task<Service> CreateService<T>(string organizationId) where T : ServiceModelBase
        {
            var type = Helpers.GetServiceType<T>();
            if (type == ServiceType.Invalid)
            {
                return null;
            }

            var service = new Service()
            {
                Id = Guid.NewGuid().ToString(),
                OrganizationId = organizationId,
                Name = $"Test Service ({type.ToString()})",
                Type = (int)type
            };
                
            await this.api.Create(service);
            return service;
        }

        protected async Task<CaseManagementData> CreateCaseManagementData(string serviceId, bool hasWaitlist, int total)
        {
            var data = new CaseManagementData()
            {
                Id = Guid.NewGuid().ToString(),
                ServiceId = serviceId,
                Name = "Test User",
                CreatedOn = DateTime.UtcNow,
                HasWaitlist = hasWaitlist,
                Total = total
            };

            await this.api.Create(data);
            return data;
        }

        private async Task InitUser(User user)
        {
            if (user != null)
            {
                // Turn context can only be accessed on a turn, so 
                // this must be called when the bot is executing a turn.
                user.Id = this.turnContext.Activity.From.Id;
                await this.api.Create(user);
            }
        }
    }
}
