using EntityModel;
using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;
using System;
using System.IO;

namespace BotTriggerHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Working...");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(BotTrigger.BotTrigger.DbModelConnectionStringSettingName);
            using (var db = DbModelFactory.Create(connectionString))
            {
                var sendTask = BotTrigger.BotTrigger.DoWork(new EfInterface(db), configuration[BotTrigger.BotTrigger.MicrosoftAppIdSettingName],
                    configuration[BotTrigger.BotTrigger.MicrosoftAppPasswordSettingName], configuration[BotTrigger.BotTrigger.BotPhoneNumberSettingName],
                    configuration[BotTrigger.BotTrigger.ChannelIdSettingName], configuration[BotTrigger.BotTrigger.ServiceUrlSettingName]);
                sendTask.Wait();
            }

            Console.WriteLine("Finished. Press 'Enter' to exit");
            Console.ReadLine();
        }
    }
}
