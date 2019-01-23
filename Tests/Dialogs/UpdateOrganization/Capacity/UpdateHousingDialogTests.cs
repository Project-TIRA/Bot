using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Valid()
        {
            var expectedOrganization = new Organization();
            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedOrganization.TotalBeds = 10;
            expectedSnapshot.OpenBeds = 5;

            // Set an initial organization.
            var initialOrganization = new Organization();
            initialOrganization.TotalBeds = expectedOrganization.TotalBeds;

            // Create the test flow.
            var testFlow = await CreateTestFlow(UpdateHousingDialog.Name, initialOrganization);

            // Execute the conversation.
            await testFlow
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Send(expectedSnapshot.OpenBeds.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task Invalid()
        {
            var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(0));

            // Create the test flow.
            var testFlow = await CreateTestFlow(UpdateHousingDialog.Name);

            // Execute the conversation.
            await testFlow
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test("5", error)
                .StartTestAsync();
        }
    }
}
