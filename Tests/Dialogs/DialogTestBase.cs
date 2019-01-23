using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot;
using ServiceProviderBot.Bot.Models.OrganizationProfile;
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
            this.state = StateAccessors.CreateFromMemoryStorage();
            this.dialogs = new DialogSet(state.DialogContextAccessor);
            this.adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(state.ConversationState))
                .Use(new AutoSaveStateMiddleware(state.OrganizationState));

            // Register prompts.
            ServiceProviderBot.Bot.Utils.Prompts.Register(this.dialogs);
        }

        protected TestFlow CreateTestFlow(string dialogName, OrganizationProfile initalOrganizationProfile = null)
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
                    if (initalOrganizationProfile != null)
                    {
                        // Set the initial organization profile.
                        await this.state.OrganizationProfileAccessor.SetAsync(
                            turnContext, initalOrganizationProfile, cancellationToken);
                    }

                    await ServiceProviderBot.Bot.Utils.Dialogs.BeginDialogAsync(this.state, this.dialogs, dialogContext, dialogName, null, cancellationToken);
                }
            });
        }

        protected OrganizationProfile CreateDefaultTestProfile()
        {
            var profile = new OrganizationProfile();
            profile.Name = TestOrgName;
            profile.Location.City = TestOrgCity;
            profile.Location.State = TestOrgState;
            profile.Location.Zip = TestOrgZip;
            profile.Demographic.Gender = Gender.All;
            profile.Demographic.AgeRange.Start = AgeRange.Default;
            profile.Demographic.AgeRange.End = AgeRange.Default;
            return profile;
        }

        protected async Task ValidateProfile(OrganizationProfile expected)
        {
            var actual = await this.state.GetOrganizationProfile(this.turnContext, this.cancellationToken);

            Assert.Equal(actual.Name, expected.Name);
            Assert.Equal(actual.Demographic.Gender, expected.Demographic.Gender);
            Assert.Equal(actual.Demographic.AgeRange.Start, expected.Demographic.AgeRange.Start);
            Assert.Equal(actual.Demographic.AgeRange.End, expected.Demographic.AgeRange.End);
            Assert.Equal(actual.Capacity.Beds.Total, expected.Capacity.Beds.Total);
            Assert.Equal(actual.Capacity.Beds.Open, expected.Capacity.Beds.Open);
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
    }
}
