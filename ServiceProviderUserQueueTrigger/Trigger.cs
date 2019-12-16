using System;
using System.Threading;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.ApiInterface;
using Shared.Storage;

namespace ServiceProviderUserQueueTrigger
{
    public static class Trigger
    {
        [FunctionName(ServiceProviderUserQueueHelper.QueueName)]
        public static async Task Run([QueueTrigger(ServiceProviderUserQueueHelper.QueueName, Connection = "AzureWebJobsStorage")]
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
                LogException(log, e);
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

                LogInfo(log, $"Sending to {user.Name} at {user.PhoneNumber}");

                MicrosoftAppCredentials.TrustServiceUrl(configuration.ServiceUrl());
                var creds = new MicrosoftAppCredentials(configuration.MicrosoftAppId(), configuration.MicrosoftAppPassword());
                var credentialProvider = new SimpleCredentialProvider(creds.MicrosoftAppId, creds.MicrosoftAppPassword);
                var adapter = new BotFrameworkAdapter(credentialProvider);
                var botAccount = new ChannelAccount() { Id = configuration.BotPhoneNumber() };
                var userAccount = new ChannelAccount() { Id = PhoneNumber.Standardize(user.PhoneNumber) };
                var convoAccount = new ConversationAccount(id: userAccount.Id);
                var convo = new ConversationReference(null, userAccount, botAccount, convoAccount, configuration.ChannelId(), configuration.ServiceUrl());

                await adapter.ContinueConversationAsync(creds.MicrosoftAppId, convo, async (context, token) =>
                {
                    var day = GetCurrentDay();
                    await context.SendActivityAsync(Phrases.Greeting.RemindToUpdate(user, day, queueData.LatestUpdateString));
                }, new CancellationToken());
            }
        }

        private static Day GetCurrentDay()
        {
            DateTime today = DateTime.UtcNow;
            switch (today.DayOfWeek)
            {
                case DayOfWeek.Sunday: return Day.Sunday;
                case DayOfWeek.Monday: return Day.Monday;
                case DayOfWeek.Tuesday: return Day.Tuesday;
                case DayOfWeek.Wednesday: return Day.Wednesday;
                case DayOfWeek.Thursday: return Day.Thursday;
                case DayOfWeek.Friday: return Day.Friday;
                case DayOfWeek.Saturday: return Day.Saturday;
                default: return Day.None;
            }
        }

        private static void LogInfo(ILogger log, string text)
        {
            if (log != null)
            {
                log.LogInformation(text);
            }
        }

        private static void LogException(ILogger log, Exception exception)
        {
            if (log != null)
            {
                log.LogError(exception, exception.Message);
            }
        }
    }
}
