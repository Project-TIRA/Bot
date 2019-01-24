using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCapacityDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UpdateAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.TotalBeds = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.OpenBeds = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateCapacityDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Send(expectedSnapshot.OpenBeds.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }
    }
}
