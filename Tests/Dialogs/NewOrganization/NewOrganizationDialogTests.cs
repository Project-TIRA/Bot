using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization
{
    public class NewOrganizationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task YesToAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.AgeRangeStart = 14;
            expectedOrganization.AgeRangeEnd = 24;
            expectedOrganization.TotalBeds = 10;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("yes", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expectedOrganization.AgeRangeStart.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Test(expectedOrganization.AgeRangeEnd.ToString(), StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expectedOrganization.TotalBeds.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoToAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoDemographic()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.TotalBeds = 10;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expectedOrganization.TotalBeds.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoAgeRange()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.TotalBeds = 10;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expectedOrganization.TotalBeds.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoHousing()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.AgeRangeStart = 14;
            expectedOrganization.AgeRangeEnd = 24;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("yes", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expectedOrganization.AgeRangeStart.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Test(expectedOrganization.AgeRangeEnd.ToString(), StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }
    }
}
