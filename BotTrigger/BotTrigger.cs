using EntityModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
            /*
            using (var dbContext = DbModelFactory.Create(connectionString))
            {
                var organizations = await dbContext.Organizations
                    .Where(o => o.IsVerified)
                    .ToListAsync();

                foreach (var organization in organizations)
                {
                */

            var organization = new Organization
            {
                PhoneNumber = "+17605004495"
            };


                    try
                    {
                        MicrosoftAppCredentials.TrustServiceUrl(ServiceUrl);

                var userAccount = new ChannelAccount() { Id = organization.PhoneNumber };
                var botAccount = new ChannelAccount() { Id = BotPhoneNumber };

                var creds = new MicrosoftAppCredentials("b89e2ca2-abdf-4263-9d93-1428a3911e49", "fkqANJED30@^{qebvKD013!");


                /*
                var creds = new MicrosoftAppCredentials("b89e2ca2-abdf-4263-9d93-1428a3911e49", "fkqANJED30@^{qebvKD013!");
                var connector = new ConnectorClient(new Uri(ServiceUrl), creds);

                List<ChannelAccount> participants = new List<ChannelAccount>();
                participants.Add(new ChannelAccount(organization.PhoneNumber));

                var botAccount = new ChannelAccount(BotPhoneNumber, BotPhoneNumber);
                var orgAccount = new ChannelAccount(organization.PhoneNumber, organization.PhoneNumber);

                var activity = new Activity()
                {
                    Type = ActivityTypes.Message,
                    Recipient = orgAccount,
                    From = botAccount,
                    Text = "TEST Create Conversation"
                };

                var param = new ConversationParameters()
                {
                    Members = new ChannelAccount[] { orgAccount },
                    Bot = botAccount,
                    Activity = activity
                };

                ConversationResourceResponse conversationId = null;

                try
                {
                    conversationId = await connector.Conversations.CreateConversationAsync(param);
                }
                catch (Exception ex)
                {
                    var a = ex;
                }

                IMessageActivity message = Activity.CreateMessageActivity();
                message.From = botAccount;
                message.Recipient = orgAccount;
                message.Conversation = new ConversationAccount(id: conversationId?.Id);
                message.ChannelId = "sms";
                message.Text = "HI!!!";
                //message.Locale = "en-Us";

                await connector.Conversations.SendToConversationAsync((Activity)message);
                */



                /*
                var credentialProvider = new SimpleCredentialProvider("b89e2ca2-abdf-4263-9d93-1428a3911e49", "fkqANJED30@^{qebvKD013!");
                        var adapter = new BotFrameworkAdapter(credentialProvider);

                var convoAccount = new ConversationAccount(id: userAccount.Id);

                var activity = new Activity()
                {
                    Type = ActivityTypes.Message,
                    Recipient = userAccount,
                    From = botAccount,
                    Text = "TEST Create Conversation"
                };

                var param = new ConversationParameters()
                {
                    Members = new ChannelAccount[] { userAccount },
                    Bot = botAccount,
                    Activity = activity
                };


                var convo = new ConversationReference("1548870074430.5957731639975665.8", userAccount, botAccount, convoAccount, "sms", ServiceUrl);

                        await adapter.ContinueConversationAsync("ServiceProviderBot", convo, async (context, token) =>
                        {
                            await context.SendActivityAsync("YES!!!");
                        }, new System.Threading.CancellationToken());
                      */  
                        

                        var connector = new ConnectorClient(new Uri(ServiceUrl), creds);
                        
                        IMessageActivity message = Activity.CreateMessageActivity();
                        message.From = botAccount;
                        message.Recipient = userAccount;
                        message.Conversation = new ConversationAccount(id: userAccount.Id);
                        message.ChannelId = "sms";
                        message.Text = "HI!!!";

                        var response = await connector.Conversations.SendToConversationAsync((Activity)message);
                        Console.WriteLine($"response:{response.Id}");
                        
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e.Message}{Environment.NewLine}{e.StackTrace}");
                    }
            /*
                }              
            }
            */
        }
    }
}
