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
        public async Task HasWaitlist()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.BedsTotal = 10;
            expectedOrganization.BedsWaitlist = true;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.BedsOpen = 0;
            expectedSnapshot.BedsWaitlist = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test(expectedSnapshot.BedsOpen.ToString(), StartsWith(Phrases.Capacity.GetHousingWaitlist))
                .Send(expectedSnapshot.BedsWaitlist.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.BedsTotal = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.BedsOpen = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Send(expectedSnapshot.BedsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task Invalid()
        {
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.BedsTotal = 10;

            var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(initialOrganization.BedsTotal));

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name, initialOrganization)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test("20", error)
                .StartTestAsync();
        }
    }
}
