using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TestBot.Bot;
using TestBot.Bot.Dialogs;
using TestBot.Bot.Models;
using Xunit;

namespace Tests.Dialogs
{
    public abstract class DialogTestBase
    {
        protected const string TestOrgName = "Test Org";

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

            // Register dialogs and prompts.
            TestBot.Bot.Utils.Dialogs.Register(this.dialogs, this.state);
            TestBot.Bot.Utils.Prompts.Register(this.dialogs);
        }

        protected TestFlow CreateTestFlow(string dialogName, OrganizationProfile initalOrganizationProfile = null)
        {
            return new TestFlow(this.adapter, async (turnContext, cancellationToken) =>
            {
                // Initialize the dialog context.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);
                DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);

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

                    await dialogContext.BeginDialogAsync(dialogName, null, cancellationToken);
                }
            });
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
