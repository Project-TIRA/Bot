using Microsoft.Extensions.Configuration;
using ServiceProviderUserQueueTrigger;
using Shared;
using Shared.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ServiceProviderOrganizationQueueHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var mainTask = Run();
            mainTask.Wait();
        }

        static async Task Run()
        {
            Console.WriteLine("Working...");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Get the next message from the queue.
            var queueHelper = new ServiceProviderUserQueueHelper(configuration.AzureWebJobsStorage());
            var message = await queueHelper.GetMessage();

            try
            {
                await Trigger.DoWork(configuration, message);
                await queueHelper.DeleteMessage(message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught: {e.Message}");
            }

            Console.WriteLine("Finished. Press 'Enter' to exit");
            Console.ReadLine();
        }
    }
}
