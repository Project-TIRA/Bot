using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TestBot.Bot;
using TestBot.Bot.Dialogs;
using Xunit;

namespace Tests.Dialogs
{
    public abstract class DialogTestBase
    {
        protected readonly StateAccessors state;
        protected readonly TestAdapter adapter;

        protected DialogTestBase()
        {
            this.state = StateAccessors.CreateFromMemoryStorage();
            this.adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(state.ConversationState))
                .Use(new AutoSaveStateMiddleware(state.OrganizationState));
        }

        protected TestFlow CreateTestFlow()
        {
            return new TestFlow(this.adapter, async (turnContext, cancellationToken) =>
            {
                // Initialize the dialog context.
                DialogContext dialogContext = await new T(this.state).CreateContextAsync(turnContext, cancellationToken);
                DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);

                if (results.Status == DialogTurnStatus.Empty)
                {
                    // Start a new conversation if there is none.
                    await dialogContext.BeginDialogAsync(MasterDialog.Name, null, cancellationToken);
                }
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
    }
}
