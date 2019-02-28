using EntityModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotTrigger
{
    public static class BotTrigger
    {
        private const string DbModelConnectionStringSettingName = "DbModel";

        // TODO: Put in settings
        private const string BotPhoneNumber = "+12066934709";
        private const string ChannelId = "sms";
        private const string ServiceUrl = "https://sms.botframework.com";

        [FunctionName("BotTrigger")]
        public static async Task Run([TimerTrigger("0 0 8 * * *")]TimerInfo myTimer, ILogger log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            log.LogInformation($"BotTrigger: executed at: {DateTime.Now}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(DbModelConnectionStringSettingName);
            await DoWork(connectionString, log);
        }

        public static async Task DoWork(string connectionString, ILogger log = null)
        {
            MicrosoftAppCredentials.TrustServiceUrl(ServiceUrl);

            var creds = new MicrosoftAppCredentials("b89e2ca2-abdf-4263-9d93-1428a3911e49", "fkqANJED30@^{qebvKD013!");
            var credentialProvider = new SimpleCredentialProvider(creds.MicrosoftAppId, creds.MicrosoftAppPassword);
            var adapter = new BotFrameworkAdapter(credentialProvider);
            var botAccount = new ChannelAccount() { Id = BotPhoneNumber };

            using (var dbContext = DbModelFactory.Create(connectionString))
            {
                var organizations = await dbContext.Organizations
                    .Where(o => o.IsVerified)
                    .ToListAsync();

                LogInfo(log, $"BotTrigger: found {organizations.Count()} verified organizations");

                foreach (var organization in organizations)
                {
                    LogInfo(log, $"BotTrigger: sending to {organization.Name}");

                    var userAccount = new ChannelAccount() { Id = organization.PhoneNumber };
                    var convoAccount = new ConversationAccount(id: userAccount.Id);
                    var convo = new ConversationReference(null, userAccount, botAccount, convoAccount, ChannelId, ServiceUrl);

                    await adapter.ContinueConversationAsync(creds.MicrosoftAppId, convo, async (context, token) =>
                    {
                        await context.SendActivityAsync(Phrases.Greeting.TimeToUpdate);
                    }, new CancellationToken());


                    /*
                    var connector = new ConnectorClient(new Uri(ServiceUrl), creds);

                    IMessageActivity message = Activity.CreateMessageActivity();
                    message.From = botAccount;
                    message.Recipient = userAccount;
                    message.Conversation = new ConversationAccount(id: userAccount.Id);
                    message.ChannelId = ChannelId;
                    message.Text = $"It's time for an update. Reply \"{Bot}";

                    await connector.Conversations.SendToConversationAsync((Activity)message);
                    */
                }
            }     
        }

        private static void LogInfo(ILogger log, string text)
        {
            if (log == null)
            {
                return;
            }

            log.LogInformation(text);
        }
    }
}
