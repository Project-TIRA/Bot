using EntityModel;
using Shared;
using Shared.ApiInterface;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DatabaseInitializer
{
    internal class Program
    {
        private const string HelpKeyword1 = "/?";
        private const string HelpKeyword2 = "-help";
        private const string CdsKeyword = "-cds";
        private const string EfKeyword = "-ef";

        private static string EfFormat = $"{EfKeyword} <environment>";

        private static string EnvironmentDevelopment = "dev";
        private static string EnvironmentStaging = "staging";

        private static string ConnectionStringDevelopment = "data source=(LocalDb)\\MSSQLLocalDB;initial catalog=ProjectTira;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        private static string ConnectionStringStaging = "Server=tcp:project-tira-staging.database.windows.net,1433;Initial Catalog=project-tira-staging;Persist Security Info=False;User ID=project-tira;Password=LamePassword1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private static async Task Main(string[] args)
        {
            if (args.Length == 0 || (args.Length == 1 && (args[0] == HelpKeyword1 || args[0] == HelpKeyword2)))
            {
                PrintHelp();
                Exit();
                return;
            }

            Console.WriteLine("Working...");

            switch (args[0])
            {
                case CdsKeyword: await HandleCds(args); break;
                case EfKeyword: await HandleEf(args); break;
                default: Debug.Assert(false); break;
            }

            Console.WriteLine("Finished!");
            Exit();
            return;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("HELP");
            Console.WriteLine($"{HelpKeyword2} : Prints this help message");
            Console.WriteLine($"{CdsKeyword} : Initializes Common Data Service");
            Console.WriteLine($"{EfFormat} : Initializes Entity Framework");

            Console.WriteLine($"Environments : {EnvironmentDevelopment}, {EnvironmentStaging}");
        }

        private static async Task HandleCds(string[] args)
        {
            await Init(new CdsInterface());
        }

        private static async Task HandleEf(string[] args)
        {
            if (args.Length != 2 || !(args[1] == EnvironmentDevelopment || args[1] == EnvironmentStaging))
            {
                Console.WriteLine($"Expected format: {EfFormat}");
                return;
            }

            var connectionString = string.Empty;

            if (args[1] == EnvironmentDevelopment)
            {
                connectionString = ConnectionStringDevelopment;
            }
            else if (args[1] == EnvironmentStaging)
            {
                connectionString = ConnectionStringStaging;
            }

            using (var db = DbModelFactory.Create(connectionString))
            {
                if (db == null)
                {
                    Console.WriteLine("Failed to create database. Invalid connection string?");
                }

                Console.WriteLine("DB should have been created heres!");
                await Init(new EfInterface(db));
            }
        }

        private static async Task Init(IApiInterface api)
        {
            for (int i = 0; i < 5; ++i)
            {
                var organization = await ServiceProviderBotTestHelpers.CreateOrganization(api, isVerified: true);
                var user = await ServiceProviderBotTestHelpers.CreateUser(api, organization.Id);

                // Randomize the services the organizations have.
                if (new Random().Next(2) == 1)
                {
                    var caseManagementService = await ServiceProviderBotTestHelpers.CreateService<CaseManagementData>(api, organization.Id);
                    var caseManagementData = await ServiceProviderBotTestHelpers.CreateCaseManagementData(api, user.Id, caseManagementService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal);
                }

                if (new Random().Next(2) == 1)
                {
                    var housingService = await ServiceProviderBotTestHelpers.CreateService<HousingData>(api, organization.Id);
                    var housingData = await ServiceProviderBotTestHelpers.CreateHousingData(api, user.Id, housingService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);
                }

                if (new Random().Next(2) == 1)
                {
                    var employmentService = await ServiceProviderBotTestHelpers.CreateService<EmploymentData>(api, organization.Id);
                    var employmentData = await ServiceProviderBotTestHelpers.CreatEmploymentData(api, user.Id, employmentService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);
                }

                if (new Random().Next(2) == 1)
                {
                    var mentalHealthService = await ServiceProviderBotTestHelpers.CreateService<MentalHealthData>(api, organization.Id);
                    var mentalHealthData = await ServiceProviderBotTestHelpers.CreateMentalHealthData(api, user.Id, mentalHealthService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);
                }

                if (new Random().Next(2) == 1)
                {
                    var substanceUseService = await ServiceProviderBotTestHelpers.CreateService<SubstanceUseData>(api, organization.Id);
                    var substanceUseData = await ServiceProviderBotTestHelpers.CreateSubstanceUseData(api, user.Id, substanceUseService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);
                }
            }
        }

        private static void Exit()
        {
            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to exit");
            Console.ReadLine();
        }
    }
}