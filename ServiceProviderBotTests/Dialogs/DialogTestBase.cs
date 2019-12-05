using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Middleware;
using Shared.Prompts;
using Xunit;

namespace ServiceProviderBotTests.Dialogs
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
            Prompt.Register(this.dialogs, this.configuration);
        }

        protected TestFlow CreateTestFlow(string dialogName, User user = null, string channelOverride = null)
        {
            return new TestFlow(this.adapter, async (turnContext, cancellationToken) =>
            {
                this.turnContext = turnContext;
                this.cancellationToken = cancellationToken;

                // Read configuration to test specific channel.
                var channelId = channelOverride ?? this.configuration.TestChannel();
                Assert.True(!string.IsNullOrEmpty(channelId));
                this.turnContext.Activity.ChannelId = channelId;

                if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    // Initialize the dialog context.
                    DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                    // Make sure this channel is supported.
                    if (!Phrases.ValidChannels.Contains(turnContext.Activity.ChannelId))
                    {
                        await Messages.SendAsync(Phrases.Greeting.InvalidChannel(turnContext), turnContext, cancellationToken);
                        return;
                    }

                    // Create the master dialog.
                    var masterDialog = new MasterDialog(this.state, this.dialogs, this.api, this.configuration);

                    // If the user sends the update keyword, clear the dialog stack and start a new update.
                    if (string.Equals(turnContext.Activity.Text, Phrases.Keywords.Update, StringComparison.OrdinalIgnoreCase))
                    {
                        dialogName = MasterDialog.Name;
                        await dialogContext.CancelAllDialogsAsync(cancellationToken);
                        await this.state.ClearUserContext(dialogContext.Context, cancellationToken);
                    }

                    // Attempt to continue any existing conversation.
                    DialogTurnResult result = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);

                    // Start a new conversation if there isn't one already.
                    if (result.Status == DialogTurnStatus.Empty)
                    {
                        // Clear the state for any new test flow. This allows tests to run multiple test flows.
                        await this.state.ClearUserContext(dialogContext.Context, cancellationToken);

                        // Tests must init the user once there is a turn context.
                        await InitUser(user);

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

        private async Task InitUser(User user)
        {
            if (user != null)
            {
                var userToken = Helpers.GetUserToken(this.turnContext);

                switch (turnContext.Activity.ChannelId)
                {
                    case Channels.Emulator: user.Name = userToken; break;
                    case Channels.Sms: user.PhoneNumber = userToken; break;
                    default: Assert.True(false, "Missing channel type"); return;
                }

                await this.api.Update(user);

                // Save the user ID and organization ID to the user context so that
                // they can be accessed by other dialogs without API lookups.
                var userContext = await this.state.GetUserContext(this.turnContext, this.cancellationToken);
                userContext.UserId = user.Id;
                userContext.OrganizationId = user.OrganizationId;
            }
        }
    }
}
