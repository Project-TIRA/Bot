using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.NewOrganization
{
    public class UpdateOrganizationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UpdateAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.TotalBeds = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.OpenBeds = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateOrganizationDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test(expectedSnapshot.OpenBeds.ToString(), Phrases.UpdateOrganization.Closing)
                .StartTestAsync();

            // Snapshot should be completed.
            expectedSnapshot.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task NothingToUpdate()
        {
            var expectedOrganization = CreateDefaultTestOrganization();

            // Execute the conversation.
            await CreateTestFlow(UpdateOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.UpdateOrganization.NothingToUpdate)
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }
    }
}
