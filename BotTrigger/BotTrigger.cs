using EntityModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.ApiInterface;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotTrigger
{
    public static class BotTrigger
    {
        public static string MicrosoftAppIdSettingName = "MicrosoftAppId";
        public static string MicrosoftAppPasswordSettingName = "MicrosoftAppPassword";
        public static string BotPhoneNumberSettingName = "BotPhoneNumber";
        public static string ChannelIdSettingName = "ChannelId";
        public static string ServiceUrlSettingName = "ServiceUrl";
        public static string DbModelConnectionStringSettingName = "DbModel";

        // TimerTrigger is in UTC - 1600 is 9am PST.
        [FunctionName("BotTrigger")]
        public static async Task Run([TimerTrigger("0 0 16 * * *")]TimerInfo myTimer, ILogger log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            log.LogInformation($"BotTrigger: executed at: {DateTime.Now}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(DbModelConnectionStringSettingName);
            using (var db = DbModelFactory.Create(connectionString))
            {
                await DoWork(new EfInterface(db), configuration[MicrosoftAppIdSettingName], configuration[MicrosoftAppPasswordSettingName],
                    configuration[BotPhoneNumberSettingName], configuration[ChannelIdSettingName], configuration[ServiceUrlSettingName], log);
            }
        }

        public static async Task DoWork(IApiInterface api, string appId, string appPassword,
            string botPhoneNumber, string channelId, string serviceUrl, ILogger log = null)
        {
            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);

            var creds = new MicrosoftAppCredentials(appId, appPassword);
            var credentialProvider = new SimpleCredentialProvider(creds.MicrosoftAppId, creds.MicrosoftAppPassword);
            var adapter = new BotFrameworkAdapter(credentialProvider);
            var botAccount = new ChannelAccount() { Id = botPhoneNumber };

            var organizations = await api.GetVerifiedOrganizations();

            LogInfo(log, $"BotTrigger: found {organizations.Count()} verified organizations");

            foreach (var organization in organizations)
            {
                var users = await api.GetUsersForOrganization(organization);
                if (users.Count == 0)
                {
                    continue;
                }

                foreach (var user in users)
                {
                    LogInfo(log, $"BotTrigger: sending to {user.Name} from {organization.Name}");

                    var userAccount = new ChannelAccount() { Id = PhoneNumber.Standardize(user.PhoneNumber) };
                    var convoAccount = new ConversationAccount(id: userAccount.Id);
                    var convo = new ConversationReference(null, userAccount, botAccount, convoAccount, channelId, serviceUrl);

                    await adapter.ContinueConversationAsync(creds.MicrosoftAppId, convo, async (context, token) =>
                    {
                        try
                        {
                            await context.SendActivityAsync(Phrases.Greeting.TimeToUpdate);
                        }
                        catch (Exception e)
                        {
                            LogException(log, e);
                        }
                    }, new CancellationToken());
                }

                //var connector = new ConnectorClient(new Uri(ServiceUrl), creds);

                //IMessageActivity message = Activity.CreateMessageActivity();
                //message.From = botAccount;
                //message.Recipient = userAccount;
                //message.Conversation = new ConversationAccount(id: userAccount.Id);
                //message.ChannelId = ChannelId;
                //message.Text = $"It's time for an update. Reply \"{Bot}";

                //await connector.Conversations.SendToConversationAsync((Activity)message);
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
