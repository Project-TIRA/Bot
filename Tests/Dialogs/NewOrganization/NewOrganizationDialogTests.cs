using System.Threading.Tasks;
using EntityModel;
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
            var expected = CreateDefaultOrganization();
            expected.AgeRangeStart = 14;
            expected.AgeRangeEnd = 24;
            expected.TotalBeds = 10;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("yes", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expected.AgeRangeStart.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Test(expected.AgeRangeEnd.ToString(), StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expected.TotalBeds.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoToAll()
        {
            var expected = CreateDefaultOrganization();

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoDemographic()
        {
            var expected = CreateDefaultOrganization();
            expected.TotalBeds = 10;

            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expected.TotalBeds.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoAgeRange()
        {
            var expected = CreateDefaultOrganization();
            expected.TotalBeds = 10;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expected.TotalBeds.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoHousing()
        {
            var expected = CreateDefaultOrganization();
            expected.AgeRangeStart = 14;
            expected.AgeRangeEnd = 24;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("yes", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expected.AgeRangeStart.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Test(expected.AgeRangeEnd.ToString(), StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }
    }
}
