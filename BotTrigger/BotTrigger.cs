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

            // Get the current day.
            var day = GetCurrentDay();

            // Get all verified organizations.
            var organizations = await api.GetVerifiedOrganizations();

            LogInfo(log, $"BotTrigger: found {organizations.Count()} verified organizations");

            foreach (var organization in organizations)
            {
                // Get all users for the organization.
                var users = await api.GetUsersForOrganization(organization);
                if (users.Count == 0)
                {
                    continue;
                }

                foreach (var user in users)
                {
                    // Make sure the user should be reminded today.
                    if (!user.ContactEnabled || !user.ReminderFrequency.HasFlag(day))
                    {
                        continue;
                    }


                    // Get the latest availability for their services as a reminder.


                    LogInfo(log, $"BotTrigger: sending to {user.Name} at {organization.Name}");

                    var userAccount = new ChannelAccount() { Id = PhoneNumber.Standardize(user.PhoneNumber) };
                    var convoAccount = new ConversationAccount(id: userAccount.Id);
                    var convo = new ConversationReference(null, userAccount, botAccount, convoAccount, channelId, serviceUrl);

                    await adapter.ContinueConversationAsync(creds.MicrosoftAppId, convo, async (context, token) =>
                    {
                        try
                        {
                            await context.SendActivityAsync(Phrases.Greeting.RemindToUpdate(user, day));
                        }
                        catch (Exception e)
                        {
                            LogException(log, e);
                        }
                    }, new CancellationToken());
                }
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
