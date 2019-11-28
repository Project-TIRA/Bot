using EntityModel;
using Shared;
using Shared.ApiInterface;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DatabaseInitializer
{
    class Program
    {
        const string HelpKeyword1 = "/?";
        const string HelpKeyword2 = "-help";
        const string CdsKeyword = "-cds";
        const string EfKeyword = "-ef";

        static string EfFormat = $"{EfKeyword} <environment>";

        static string EnvironmentDevelopment = "dev";
        static string EnvironmentStaging = "staging";

        static string ConnectionStringDevelopment = "data source=(LocalDb)\\MSSQLLocalDB;initial catalog=ProjectTira;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        static string ConnectionStringStaging = "Server=tcp:project-tira-staging.database.windows.net,1433;Initial Catalog=project-tira-staging;Persist Security Info=False;User ID=project-tira;Password=LamePassword1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        static async Task Main(string[] args)
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

        static void PrintHelp()
        {
            Console.WriteLine("HELP");
            Console.WriteLine($"{HelpKeyword2} : Prints this help message");
            Console.WriteLine($"{CdsKeyword} : Initializes Common Data Service");
            Console.WriteLine($"{EfFormat} : Initializes Entity Framework");

            Console.WriteLine($"Environments : {EnvironmentDevelopment}, {EnvironmentStaging}");
        }

        static async Task HandleCds(string[] args)
        {
            // TODO: Pass configuration with CDS params.
            await Init(new CdsInterface(null));
        }

        static async Task HandleEf(string[] args)
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

                await Init(new EfInterface(db));
            }
        }

        static async Task Init(IApiInterface api)
        {
            for (int i = 0; i < 5; ++i)
            {
                var organization = await TestHelpers.CreateOrganization(api, isVerified: true);
                var user = await TestHelpers.CreateUser(api, organization.Id);

                // Randomize the services the organizations have.
                if (new Random().Next(2) == 1)
                {
                    var caseManagementService = await TestHelpers.CreateService<CaseManagementData>(api, organization.Id);
                    var caseManagementData = await TestHelpers.CreateCaseManagementData(api, user.Id, caseManagementService.Id);
                }

                if (new Random().Next(2) == 1)
                {
                    var housingService = await TestHelpers.CreateService<HousingData>(api, organization.Id);
                    var housingData = await TestHelpers.CreateHousingData(api, user.Id, housingService.Id);
                }

                if (new Random().Next(2) == 1)
                {
                    var employmentService = await TestHelpers.CreateService<EmploymentData>(api, organization.Id);
                    var employmentData = await TestHelpers.CreatEmploymentData(api, user.Id, employmentService.Id);
                }

                if (new Random().Next(2) == 1)
                {
                    var mentalHealthService = await TestHelpers.CreateService<MentalHealthData>(api, organization.Id);
                    var mentalHealthData = await TestHelpers.CreateMentalHealthData(api, user.Id, mentalHealthService.Id);
                }

                if (new Random().Next(2) == 1)
                {
                    var substanceUseService = await TestHelpers.CreateService<SubstanceUseData>(api, organization.Id);
                    var substanceUseData = await TestHelpers.CreateSubstanceUseData(api, user.Id, substanceUseService.Id);
                }
            }
        }

        static void Exit()
        {
            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to exit");
            Console.ReadLine();
        }
    }
}
