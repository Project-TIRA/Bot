using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Demographic;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Demographic
{
    public class DemographicDialogTests : DialogTestBase
    {
        [Fact]
        public async Task MenOnly()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.Gender = Gender.Male;

            // Execute the conversation.
            await CreateTestFlow(DemographicDialog.Name, expectedOrganization)
                .Test("begin", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("no", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Send("no")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task WomenOnly()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.Gender = Gender.Female;

            // Execute the conversation.
            await CreateTestFlow(DemographicDialog.Name, expectedOrganization)
                .Test("begin", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("no", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Send("no")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task GenderUnknown()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.Gender = Gender.Unknown;

            // Execute the conversation.
            await CreateTestFlow(DemographicDialog.Name, expectedOrganization)
                .Test("begin", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("no", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("no", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Send("no")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }
    }
}
