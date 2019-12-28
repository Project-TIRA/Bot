using EntityModel;
using EntityModel.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.ApiInterface;
using Shared.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceProviderTriggers
{
    public static class TimerTrigger
    {
        [FunctionName(nameof(TimerTrigger))]
        public static async Task Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
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
                Helpers.LogException(log, e);
                throw e;
            }
        }

        public static async Task DoWork(IConfiguration configuration, ILogger log = null)
        {
            using (var db = DbModelFactory.Create(configuration.DbModelConnectionString()))
            {
                var api = new EfInterface(db);

                // Get all verified organizations.
                var organizations = await api.GetVerifiedOrganizations();
                Helpers.LogInfo(log, $"Found {organizations.Count()} verified organizations");

                var queueHelper = new ServiceProviderOrganizationQueueHelpers(configuration.AzureWebJobsStorage());

                foreach (var organization in organizations)
                {
                    var data = new ServiceProviderOrganizationQueueData();
                    data.OrganizationId = organization.Id;

                    // Get all users for the organization.
                    var users = await api.GetUsersForOrganization(organization);

                    foreach (var user in users)
                    {
                        DateTime localDateTime = DateTime.UtcNow.AddHours(user.TimezoneOffset);
                        var localDay = DayFlagsHelpers.FromDateTime(localDateTime);

                        Helpers.LogInfo(log, $"User: {user.Name}, organization: {organization.Name}, " +
                            $"contact enabled: {user.ContactEnabled}, reminder days: {user.ReminderFrequency}, current local day: {localDay}");

                        // Check if the user should be reminded today.
                        if (!user.ContactEnabled || !user.ReminderFrequency.HasFlag(localDay))
                        {
                            continue;
                        }

                        // Check if the user should be reminded at this time.
                        // Special case: use 5pm UTC (9am PST) if their reminder time isn't set.
                        DateTime userReminderTime = string.IsNullOrEmpty(user.ReminderTime) ?
                            DateTime.Parse("5:00pm") : DateTime.Parse(user.ReminderTime);

                        // Adjust the time by their timezone offset to compare.
                        var localTime = DateTime.UtcNow.AddHours(user.TimezoneOffset);

                        Helpers.LogInfo(log, $"Reminder time: {user.ReminderTime}, current local time: {DateTime.UtcNow.AddHours(user.TimezoneOffset).ToShortTimeString()}");
                        Helpers.LogInfo(log, $"Time diff minutes: {Math.Abs((localTime - userReminderTime).TotalMinutes)}");

                        // Using a 5 minute window in case the function triggers slightly early or late.
                        if (Math.Abs((localTime - userReminderTime).TotalMinutes) > 5)
                        {
                            continue;
                        }

                        Helpers.LogInfo(log, $"Adding {user.Name} from {organization.Name} to queue");

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
    }
}
