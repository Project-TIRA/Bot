using System;
using System.Threading;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.Dialogs;
using SearchBot.Bot.State;
using Shared.ApiInterface;
using Shared.Middleware;
using Shared.Prompts;
using Xunit;

namespace SearchBotTests.Dialogs
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
                .Use(new TrimIncomingMessageMiddleware())
                .Use(new AutoSaveStateMiddleware(state.ConversationState));

            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true).Build();

            // Register prompts.
            Prompt.Register(this.dialogs);
        }

        protected TestFlow CreateTestFlow(string dialogName, ConversationContext conversationContext = null)
        {
            return new TestFlow(this.adapter, async (turnContext, cancellationToken) =>
            {
                this.turnContext = turnContext;
                this.cancellationToken = cancellationToken;

                if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    // Initialize the dialog context.
                    DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                    // Create the master dialog.
                    var masterDialog = new MasterDialog(this.state, this.dialogs, this.api, this.configuration);

                    // Attempt to continue any existing conversation.
                    DialogTurnResult result = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);

                    // Start a new conversation if there isn't one already.
                    if (result.Status == DialogTurnStatus.Empty)
                    {
                        // Tests must init the conversation context once there is a turn context.
                        await InitConversationContext(conversationContext);

                        // Difference for tests here is beginning the given dialog instead of master so that individual dialog flows can be tested.
                        await masterDialog.BeginDialogAsync(dialogContext, dialogName, null, cancellationToken);
                    }
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

        private async Task InitConversationContext(ConversationContext conversationContext)
        {
            if (conversationContext != null)
            {
                await this.state.ConversationContextAccessor.SetAsync(this.turnContext, conversationContext, this.cancellationToken);
            }
        }
    }
}
