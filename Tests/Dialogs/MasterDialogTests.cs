using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using TestBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task YesToAll()
        {
            await CreateTestFlow()
                .Test("hello", Phrases.NewOrganization.GetName)
                .Test("test org", StartsWith(Phrases.NewOrganization.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.NewOrganization.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.NewOrganization.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.NewOrganization.GetHasDemographicAgeRange))
                .Test("yes", Phrases.NewOrganization.GetAgeRangeStart)
                .Test("14", Phrases.NewOrganization.GetAgeRangeEnd)
                .Test("24", StartsWith(Phrases.NewOrganization.GetHasHousing))
                .Test("yes", Phrases.Shared.GetHousingTotal)
                .Test("10", Phrases.Shared.GetHousingOpen)
                .Test("5", Phrases.Shared.GetClosing)
                .StartTestAsync();
        }

        [Fact]
        public async Task NoDemographic()
        {
            await CreateTestFlow()
                .Test("hello", Phrases.NewOrganization.GetName)
                .Test("test org", StartsWith(Phrases.NewOrganization.GetHasDemographic))
                .Test("no", StartsWith(Phrases.NewOrganization.GetHasHousing))
                .Test("yes", Phrases.Shared.GetHousingTotal)
                .Test("10", Phrases.Shared.GetHousingOpen)
                .Test("5", Phrases.Shared.GetClosing)
                .StartTestAsync();
        }

        [Fact]
        public async Task NoAgeRange()
        {
            await CreateTestFlow()
                .Test("hello", Phrases.NewOrganization.GetName)
                .Test("test org", StartsWith(Phrases.NewOrganization.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.NewOrganization.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.NewOrganization.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.NewOrganization.GetHasDemographicAgeRange))
                .Test("no", StartsWith(Phrases.NewOrganization.GetHasHousing))
                .Test("yes", Phrases.Shared.GetHousingTotal)
                .Test("10", Phrases.Shared.GetHousingOpen)
                .Test("5", Phrases.Shared.GetClosing)
                .StartTestAsync();
        }

        [Fact]
        public async Task NoHousing()
        {
            await CreateTestFlow()
                .Test("hello", Phrases.NewOrganization.GetName)
                .Test("test org", StartsWith(Phrases.NewOrganization.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.NewOrganization.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.NewOrganization.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.NewOrganization.GetHasDemographicAgeRange))
                .Test("yes", Phrases.NewOrganization.GetAgeRangeStart)
                .Test("14", Phrases.NewOrganization.GetAgeRangeEnd)
                .Test("24", StartsWith(Phrases.NewOrganization.GetHasHousing))
                .Test("no", Phrases.Shared.GetClosing)
                .StartTestAsync();
        }
    }
}
