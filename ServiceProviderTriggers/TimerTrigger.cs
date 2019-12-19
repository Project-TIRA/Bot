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

                // Get the current day.
                var day = DayFlagsHelpers.CurrentDay();

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
                        // Check if the user should be reminded today.
                        if (!user.ContactEnabled || !user.ReminderFrequency.HasFlag(day))
                        {
                            continue;
                        }

                        // Check if the user should be reminded at this time.
                        // Special case: use 6pm UTC (10am PST) if their reminder time isn't set.
                        DateTime userReminderTime = string.IsNullOrEmpty(user.ReminderTime) ?
                            DateTime.Parse("6:00pm") : DateTime.Parse(user.ReminderTime);

                        // Using a 5 minute window in case the function triggers slightly early or late.
                        if (Math.Abs((DateTime.UtcNow - userReminderTime).TotalMinutes) > 5)
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
