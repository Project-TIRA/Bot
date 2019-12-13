using Microsoft.Extensions.Configuration;
using ServiceProviderTimerTrigger;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ServiceProviderTimerTriggerHarness
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

            try
            {
                await Trigger.DoWork(configuration);
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
