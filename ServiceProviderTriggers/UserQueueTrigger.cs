using System;
using System.Threading;
using System.Threading.Tasks;
using EntityModel;
using EntityModel.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.ApiInterface;
using Shared.Storage;

namespace ServiceProviderTriggers
{
    public static class UserQueueTrigger
    {
        [FunctionName(nameof(UserQueueTrigger))]
        public static async Task Run([QueueTrigger(ServiceProviderUserQueueHelpers.QueueName, Connection = "AzureWebJobsStorage")]
            ServiceProviderUserQueueData queueData, ILogger log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                await DoWork(configuration, queueData, log);
            }
            catch (Exception e)
            {
                Helpers.LogException(log, e);
                throw e;
            }
        }

        public static async Task DoWork(IConfiguration configuration, ServiceProviderUserQueueData queueData, ILogger log = null)
        {
            using (var db = DbModelFactory.Create(configuration.DbModelConnectionString()))
            {
                var api = new EfInterface(db);

                // Get the user.
                var user = await api.GetUser(queueData.UserId);
                if (user == null)
                {
                    return;
                }

                Helpers.LogInfo(log, $"Sending to {user.Name} at {user.PhoneNumber}");

                MicrosoftAppCredentials.TrustServiceUrl(configuration.ServiceUrl());
                var creds = new MicrosoftAppCredentials(configuration.MicrosoftAppId(), configuration.MicrosoftAppPassword());
                var credentialProvider = new SimpleCredentialProvider(creds.MicrosoftAppId, creds.MicrosoftAppPassword);
                var adapter = new BotFrameworkAdapter(credentialProvider);
                var botAccount = new ChannelAccount() { Id = configuration.BotPhoneNumber() };
                var userAccount = new ChannelAccount() { Id = PhoneNumberHelpers.Standardize(user.PhoneNumber) };
                var convoAccount = new ConversationAccount(id: userAccount.Id);
                var convo = new ConversationReference(null, userAccount, botAccount, convoAccount, configuration.ChannelId(), configuration.ServiceUrl());

                await adapter.ContinueConversationAsync(creds.MicrosoftAppId, convo, async (context, token) =>
                {
                    // TODO: Current day gives UTC, so it can be wrong for the user's local day.
                    // Might need to keep user's timezone offset for this.
                    var day = DayFlagsHelpers.CurrentDay(user);
                    await context.SendActivityAsync(Phrases.Greeting.RemindToUpdate(user, day, queueData.LatestUpdateString));
                }, new CancellationToken());
            }
        }
    }
}
