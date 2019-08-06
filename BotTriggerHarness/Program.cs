using EntityModel;
using Shared.ApiInterface;
using System;

namespace BotTriggerHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=tcp:project-tira-staging.database.windows.net,1433;Initial Catalog=project-tira-staging;Persist Security Info=False;User ID=project-tira;Password=LamePassword1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //var connectionString = "data source=(LocalDb)\\MSSQLLocalDB;initial catalog=ProjectTira;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";

            using (var db = DbModelFactory.Create(connectionString))
            {
                var sendTask = BotTrigger.BotTrigger.DoWork(new EfInterface(db));
                sendTask.Wait();
            }

            Console.WriteLine("Press 'Enter' to quit");
            Console.ReadLine();
        }
    }
}
