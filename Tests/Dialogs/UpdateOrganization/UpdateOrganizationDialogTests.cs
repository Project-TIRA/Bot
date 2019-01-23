using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization
{
    public class UpdateOrganizationDialogTests : DialogTestBase
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
            await CreateTestFlow(UpdateOrganizationDialog.Name, initialOrganization)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test(expectedSnapshot.OpenBeds.ToString(), Phrases.UpdateOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task NothingToUpdate()
        {
            var expectedOrganization = new Organization();

            // Execute the conversation.
            await CreateTestFlow(UpdateOrganizationDialog.Name)
                .Test("begin", Phrases.UpdateOrganization.NothingToUpdate)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expectedOrganization);
        }
    }
}
