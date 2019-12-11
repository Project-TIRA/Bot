using System;
using System.Collections.Generic;
using System.Linq;
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
using Shared;
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
        protected readonly IConfiguration configuration;

        protected ITurnContext turnContext;
        protected CancellationToken cancellationToken;

        public static IEnumerable<object[]> TestTypes => Helpers.GetServiceDataTypes().Select(t => new object[] { t });
        public static IEnumerable<object[]> TestTypePairs => Helpers.GetServiceDataTypes().SelectMany((t, i) => Helpers.GetServiceDataTypes().Skip(i+1), (t1, t2) => new object[] { t1, t2 });

        public static IEnumerable<object[]> TestFlags => Enum.GetValues(typeof(ServiceFlags)).OfType<ServiceFlags>().Select(f => new object[] { f });
        public static IEnumerable<object[]> TestFlagPairs => Enum.GetValues(typeof(ServiceFlags)).OfType<ServiceFlags>().SelectMany((f, i) => Enum.GetValues(typeof(ServiceFlags)).OfType<ServiceFlags>().Skip(i + 1), (f1, f2) => new object[] { f1, f2 });

        protected DialogTestBase()
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true).Build();

            this.state = StateAccessors.Create();
            this.dialogs = new DialogSet(state.DialogContextAccessor);
            this.api = new EfInterface(DbModelFactory.CreateInMemory());

            this.adapter = new TestAdapter()
                .Use(new TrimIncomingMessageMiddleware())
                .Use(new AutoSaveStateMiddleware(state.ConversationState));

            // Register prompts.
            Prompt.Register(this.dialogs, this.configuration);
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
                        // Clear the conversation context when a new conversation begins.
                        await this.state.ClearConversationContext(dialogContext.Context, cancellationToken);

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
