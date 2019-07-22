using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Valid()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.TotalBeds = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.OpenBeds = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Send(expectedSnapshot.OpenBeds.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task Invalid()
        {
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.TotalBeds = 10;

            var error = Phrases.Capacity.GetHousingError(initialOrganization.TotalBeds);

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name, initialOrganization)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test("20", error)
                .StartTestAsync();
        }
    }
}
