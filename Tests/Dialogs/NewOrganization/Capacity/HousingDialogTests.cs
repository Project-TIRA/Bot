using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Capacity
{
    public class HousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task YesToAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.BedsTotal = 10;
            expectedOrganization.BedsWaitlist = true;

            // Execute the conversation.
            await CreateTestFlow(HousingDialog.Name, expectedOrganization)
                .Test("begin", Phrases.Capacity.GetHousingTotal)
                .Test(expectedOrganization.BedsTotal.ToString(), StartsWith(Phrases.Capacity.GetHasHousingWaitlist))
                .Send("yes")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.BedsTotal = 10;
            expectedOrganization.BedsWaitlist = false;

            // Execute the conversation.
            await CreateTestFlow(HousingDialog.Name, expectedOrganization)
                .Test("begin", Phrases.Capacity.GetHousingTotal)
                .Test(expectedOrganization.BedsTotal.ToString(), StartsWith(Phrases.Capacity.GetHasHousingWaitlist))
                .Send("no")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task Invalid()
        {
            var initialOrganization = CreateDefaultTestOrganization();

            // Execute the conversation.
            await CreateTestFlow(HousingDialog.Name, initialOrganization)
                .Test("begin", Phrases.Capacity.GetHousingTotal)
                .Test("lots", Phrases.Capacity.GetHousingTotal)
                .StartTestAsync();
        }
    }
}
