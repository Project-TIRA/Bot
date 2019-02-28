using System;
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
using Xunit;

namespace Tests.Dialogs
{
    public abstract class DialogTestBase
    {
        protected const string TestOrgName = "Test Org";
        protected const string TestOrgCity = "Redmond";
        protected const string TestOrgState = "WA";
        protected const string TestOrgZip = "98052";

        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly DbInterface database;
        protected readonly TestAdapter adapter;
        private readonly IConfiguration configuration;

        protected ITurnContext turnContext;
        protected CancellationToken cancellationToken;

        protected DialogTestBase()
        {
            this.state = StateAccessors.Create();
            this.dialogs = new DialogSet(state.DialogContextAccessor);
            this.database = new DbInterface(DbModelFactory.CreateInMemory());
            this.adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(state.ConversationState));

            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true).Build();

            // Register prompts.
            Prompts.Register(this.state, this.dialogs, this.database);
        }

        protected TestFlow CreateTestFlow(string dialogName, Organization initialOrganization = null, Snapshot initialSnapshot = null)
        {
            return new TestFlow(this.adapter, async (turnContext, cancellationToken) =>
            {
                this.turnContext = turnContext;
                this.cancellationToken = cancellationToken;

                // Initialize the dialog context.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                // Create the master dialog.
                var masterDialog = new MasterDialog(this.state, this.dialogs, this.database, this.configuration);

                // Attempt to continue any existing conversation.
                DialogTurnResult results = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);
                var startNewConversation = turnContext.Activity.Type == ActivityTypes.Message && results.Status == DialogTurnStatus.Empty;

                if (startNewConversation)
                {
                    // Init at the start of the conversation. Need to do before checking for expired data.
                    await InitDatabase(turnContext, initialOrganization, initialSnapshot);
                }

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
            });
        }

        protected Organization CreateDefaultTestOrganization()
        {
            var organization = new Organization();
            organization.Name = TestOrgName;
            organization.Gender = Gender.All;
            organization.City = TestOrgCity;
            organization.State = TestOrgState;
            organization.Zip = TestOrgZip;
            return organization;
        }

        protected async Task ValidateProfile(Organization expectedOrganization = null, Snapshot expectedSnapshot = null)
        {
            if (expectedOrganization != null)
            {
                var actualOrganization = await this.database.GetOrganization(this.turnContext);
                Assert.Equal(actualOrganization.Name, expectedOrganization.Name);
                Assert.Equal(actualOrganization.Gender, expectedOrganization.Gender);
                Assert.Equal(actualOrganization.AgeRangeStart, expectedOrganization.AgeRangeStart);
                Assert.Equal(actualOrganization.AgeRangeEnd, expectedOrganization.AgeRangeEnd);
                Assert.Equal(actualOrganization.TotalBeds, expectedOrganization.TotalBeds);
            }

            if (expectedSnapshot != null)
            {
                var actualSnapshot = await this.database.GetSnapshot(this.turnContext);
                Assert.Equal(actualSnapshot.OpenBeds, expectedSnapshot.OpenBeds);
            }
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

        private async Task InitDatabase(ITurnContext turnContext, Organization initialOrganization = null, Snapshot initialSnapshot = null)
        {
            Assert.True(initialSnapshot == null || initialOrganization != null, "Cannot initialize a snapshot without an organization");

            // Create the organization and snapshot if provided.
            if (initialOrganization != null)
            {
                var organization = await this.database.CreateOrganization(turnContext);
                organization.DateCreated = initialOrganization.DateCreated;
                organization.IsVerified = initialOrganization.IsVerified;
                organization.Name = initialOrganization.Name;
                organization.City = initialOrganization.City;
                organization.State = initialOrganization.State;
                organization.Zip = initialOrganization.Zip;
                organization.Gender = initialOrganization.Gender;
                organization.AgeRangeStart = initialOrganization.AgeRangeStart;
                organization.AgeRangeEnd = initialOrganization.AgeRangeEnd;
                organization.TotalBeds = initialOrganization.TotalBeds;

                if (initialSnapshot != null)
                {
                    var snapshot = await this.database.CreateSnapshot(turnContext);
                    snapshot.Date = initialSnapshot.Date;
                    snapshot.OpenBeds = initialSnapshot.OpenBeds;
                }

                await this.database.Save();
            }
        }
    }
}
