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
        [FunctionName(ServiceProviderOrganizationQueueHelpers.QueueName)]
        public static async Task Run([QueueTrigger(ServiceProviderOrganizationQueueHelpers.QueueName, Connection = "AzureWebJobsStorage")]
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
                Helpers.LogException(log, e);
                throw e;
            }
        }

        public static async Task DoWork(IConfiguration configuration, ServiceProviderOrganizationQueueData queueData, ILogger log = null)
        {
            using (var db = DbModelFactory.Create(configuration.DbModelConnectionString()))
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

                var queueHelper = new ServiceProviderUserQueueHelpers(configuration.AzureWebJobsStorage());

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
    }
}
