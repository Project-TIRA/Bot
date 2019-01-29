using System;
using System.Threading.Tasks;

namespace BotTriggerHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "data source=(LocalDb)\\MSSQLLocalDB;initial catalog=OrganizationModel;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            var sendTask = BotTrigger.BotTrigger.Send(connectionString);
            sendTask.Wait();

            Console.WriteLine("Press 'enter' to quit");
            Console.ReadLine();
        }
    }
}
