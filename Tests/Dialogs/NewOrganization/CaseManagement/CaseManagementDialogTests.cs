using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.CaseManagement;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.NewOrganization.CaseManagement
{
    public class CaseManagementTests : DialogTestBase
    {
        [Fact]
        public async Task NoCaseManagement()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.CaseManagementTotal = 0;

            // Execute the conversation.
            await CreateTestFlow(CaseManagementDialog.Name, expectedOrganization)
                .Test("begin", StartsWith(Phrases.CaseManagement.GetHasCaseManagement))
                .Send("no")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task YesToAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.CaseManagementTotal = 20;
            expectedOrganization.CaseManagementHasWaitlist = true;
            expectedOrganization.CaseManagementGender = Gender.All;
            expectedOrganization.CaseManagementAgeRangeStart = 10;
            expectedOrganization.CaseManagementAgeRangeEnd = 40;
            expectedOrganization.CaseManagementSobriety = true;

            await CreateTestFlow(CaseManagementDialog.Name, expectedOrganization)
                .Test("begin", StartsWith(Phrases.CaseManagement.GetHasCaseManagement))
                .Test("yes", StartsWith(Phrases.CaseManagement.GetCaseManagementTotal))
                .Test(expectedOrganization.CaseManagementTotal.ToString(), StartsWith(Phrases.CaseManagement.GetHasWaitingList))
                .Test("yes", StartsWith(Phrases.CaseManagement.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.CaseManagement.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.CaseManagement.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.CaseManagement.GetHasDemographicAgeRange))
                .Test("yes", StartsWith(Phrases.CaseManagement.GetAgeRangeStart))
                .Test(expectedOrganization.CaseManagementAgeRangeStart.ToString(), StartsWith(Phrases.CaseManagement.GetAgeRangeEnd))
                .Test(expectedOrganization.CaseManagementAgeRangeEnd.ToString(), StartsWith(Phrases.CaseManagement.GetHasSobriety))
                .Send("yes")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }
    }
}
