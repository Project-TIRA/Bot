using System;
using System.Threading;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using ServiceProviderBot.Bot;
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
        protected readonly TestAdapter adapter;

        protected ITurnContext turnContext;
        protected CancellationToken cancellationToken;

        protected DialogTestBase()
        {
            // Create an in-memory DB for each test class so that tests can run async without collisions.
            var dbContext = DbModelFactory.CreateInMemory();

            this.state = StateAccessors.Create(dbContext);
            this.dialogs = new DialogSet(state.DialogContextAccessor);
            this.adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(state.ConversationState));

            // Register prompts.
            ServiceProviderBot.Bot.Utils.Prompts.Register(this.dialogs);
        }

        protected TestFlow CreateTestFlow(string dialogName, Organization initialOrganization = null, Snapshot initialSnapshot = null)
        {
            return new TestFlow(this.adapter, async (turnContext, cancellationToken) =>
            {
                // Initialize the dialog context.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);
                DialogTurnResult results = await ServiceProviderBot.Bot.Utils.Dialogs.ContinueDialogAsync(this.state, this.dialogs, dialogContext, cancellationToken);

                this.turnContext = turnContext;
                this.cancellationToken = cancellationToken;

                // Start the dialog if there is no conversation.
                if (results.Status == DialogTurnStatus.Empty)
                {
                    // Init at the start of the conversation.
                    await InitDatabase(turnContext, initialOrganization, initialSnapshot);

                    await ServiceProviderBot.Bot.Utils.Dialogs.BeginDialogAsync(this.state, this.dialogs, dialogContext, dialogName, null, cancellationToken);
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
                var actualOrganization = await this.state.GetOrganization(this.turnContext);
                Assert.Equal(actualOrganization.Name, expectedOrganization.Name);
                Assert.Equal(actualOrganization.Gender, expectedOrganization.Gender);
                Assert.Equal(actualOrganization.AgeRangeStart, expectedOrganization.AgeRangeStart);
                Assert.Equal(actualOrganization.AgeRangeEnd, expectedOrganization.AgeRangeEnd);
                Assert.Equal(actualOrganization.TotalBeds, expectedOrganization.TotalBeds);
            }

            if (expectedSnapshot != null)
            {
                var actualSnapshot = await this.state.GetSnapshot(this.turnContext);
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

        private async Task InitDatabase(ITurnContext context, Organization initialOrganization = null, Snapshot initialSnapshot = null)
        {
            Assert.True(initialSnapshot == null || initialOrganization != null, "Cannot initialize a snapshot without an organization");

            // Create the organization and snapshot if provided.
            if (initialOrganization != null)
            {
                var organization = await this.state.CreateOrganization(context);
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
                    var snapshot = await this.state.CreateSnapshot(context);
                    snapshot.OpenBeds = initialSnapshot.OpenBeds;
                }

                await this.state.SaveDbContext();
            }
        }
    }
}
