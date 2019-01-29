using EntityModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotTrigger
{
    public static class BotTrigger
    {
        private const string DbModelConnectionStringSettingName = "DbModel";

        // TODO: Put in settings
        private const string BotPhoneNumber = "+12066934709";
        private const string ChannelId = "sms";
        private const string ServiceUrl = "https://sms.botframework.com/api/sms";

        [FunctionName("BotTrigger")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(DbModelConnectionStringSettingName);
            await Send(connectionString);
        }

        public static async Task Send(string connectionString)
        {
            using (var dbContext = DbModelFactory.Create(connectionString))
            {
                var organizations = await dbContext.Organizations
                    .Where(o => o.IsVerified)
                    .ToListAsync();

                foreach (var organization in organizations)
                {
                    try
                    {
                        var userAccount = new ChannelAccount() { Id = organization.PhoneNumber };
                        var botAccount = new ChannelAccount() { Id = BotPhoneNumber };

                        MicrosoftAppCredentials.TrustServiceUrl(ServiceUrl);
                        var account = new MicrosoftAppCredentials("b89e2ca2-abdf-4263-9d93-1428a3911e49", "fkqANJED30@^{qebvKD013!");
                        var connector = new ConnectorClient(new Uri(ServiceUrl), account);

                        IMessageActivity message = Activity.CreateMessageActivity();
                        message.Text = "Message sent from console application!!!";

                        var conversation = await connector.Conversations.CreateDirectConversationAsync(userAccount, botAccount);
                        var response = await connector.Conversations.SendToConversationAsync((Activity)message);
                        Console.WriteLine($"response:{response.Id}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e.Message}{Environment.NewLine}{e.StackTrace}");
                    }
                }
            }
        }
    }
}
