using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Capacity
{
    public class CapacityDialogTests : DialogTestBase
    {
        [Fact]
        public async Task YesToAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.BedsTotal = 10;
            expectedOrganization.BedsWaitlist = true;

            // Execute the conversation.
            await CreateTestFlow(CapacityDialog.Name, expectedOrganization)
                .Test("begin", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expectedOrganization.BedsTotal.ToString(), StartsWith(Phrases.Capacity.GetHasHousingWaitlist))
                .Send("yes")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoHousing()
        {
            var expectedOrganization = CreateDefaultTestOrganization();

            // Execute the conversation.
            await CreateTestFlow(CapacityDialog.Name, expectedOrganization)
                .Test("begin", StartsWith(Phrases.Capacity.GetHasHousing))
                .Send("no")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }
    }
}
