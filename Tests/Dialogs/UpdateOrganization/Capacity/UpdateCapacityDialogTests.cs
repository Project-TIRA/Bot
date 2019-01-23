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
            var expectedOrganization = new Organization();
            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedOrganization.TotalBeds = 10;
            expectedSnapshot.OpenBeds = 5;

            // Set an initial organization.
            var initialOrganization = new Organization();
            initialOrganization.TotalBeds = expectedOrganization.TotalBeds;

            // Execute the conversation.
            await CreateTestFlow(UpdateCapacityDialog.Name, initialOrganization)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Send(expectedSnapshot.OpenBeds.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }
    }
}
