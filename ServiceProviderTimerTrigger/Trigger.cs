using EntityModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.ApiInterface;
using Shared.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceProviderTimerTrigger
{
    public static class Trigger
    {
        public static string DbModelConnectionStringSettingName = "DbModel";

        // TimerTrigger is in UTC - 1600 is 9am PST.
        [FunctionName("ServiceProviderBotTrigger")]
        public static async Task Run([TimerTrigger("0 0 16 * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation($"Executed at: {DateTime.Now}");

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                await DoWork(configuration, log);
            }
            catch(Exception e)
            {
                LogException(log, e);
                throw e;
            }
        }

        public static async Task DoWork(IConfiguration configuration, ILogger log = null)
        {
            var connectionString = configuration.GetConnectionString(DbModelConnectionStringSettingName);
            var queueHelper = new ServiceProviderOrganizationQueueHelper(configuration.AzureWebJobsStorage());

            using (var db = DbModelFactory.Create(connectionString))
            {
                var api = new EfInterface(db);

                // Get the current day.
                var day = GetCurrentDay();

                // Get all verified organizations.
                var organizations = await api.GetVerifiedOrganizations();
                LogInfo(log, $"Found {organizations.Count()} verified organizations");

                foreach (var organization in organizations)
                {
                    var data = new ServiceProviderOrganizationQueueData();
                    data.OrganizationId = organization.Id;

                    // Get all users for the organization.
                    var users = await api.GetUsersForOrganization(organization);

                    foreach (var user in users)
                    {
                        // Check if the user should be reminded today.
                        if (!user.ContactEnabled || !user.ReminderFrequency.HasFlag(day))
                        {
                            continue;
                        }

                        // TODO: Check if the user should be reminded at this time.

                        data.UserIds.Add(user.Id);
                    }

                    if (data.UserIds.Count > 0)
                    {
                        // Add to the queue to process.
                        await queueHelper.AddMessage(data);
                    }
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
