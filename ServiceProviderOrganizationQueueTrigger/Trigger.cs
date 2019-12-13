using System;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.ApiInterface;
using Shared.Storage;

namespace ServiceProviderOrganizationQueueTrigger
{
    public static class Trigger
    {
        public static string DbModelConnectionStringSettingName = "DbModel";

        [FunctionName(ServiceProviderOrganizationQueueHelper.QueueName)]
        public static async Task Run([QueueTrigger(ServiceProviderOrganizationQueueHelper.QueueName, Connection = "AzureWebJobsStorage")]
            ServiceProviderOrganizationQueueData queueData, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation($"Executed at: {DateTime.Now}");

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

        public static async Task DoWork(IConfiguration configuration, ServiceProviderOrganizationQueueData queueData, ILogger log = null)
        {
            var connectionString = configuration.GetConnectionString(DbModelConnectionStringSettingName);
            var queueHelper = new ServiceProviderUserQueueHelper(configuration.AzureWebJobsStorage());

            using (var db = DbModelFactory.Create(connectionString))
            {
                var api = new EfInterface(db);

                // Get the latest update details for the organization.
                var latestUpdateString = string.Empty;

                foreach (var dataType in Helpers.GetServiceDataTypes())
                {
                    var data = await api.GetLatestServiceData(queueData.OrganizationId, dataType);
                    if (data != null)
                    {
                        latestUpdateString += data.ToString();
                    }
                }

                foreach (var userId in queueData.UserIds)
                {
                    var data = new ServiceProviderUserQueueData()
                    {
                        UserId = userId,
                        LatestUpdateString = latestUpdateString
                    };

                    // Add to the queue to process.
                    await queueHelper.Enqueue(data);
                }
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
