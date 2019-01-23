using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Capacity
{
    public class CapacityDialogTests : DialogTestBase
    {
        [Fact]
        public async Task YesToAll()
        {
            var expected = new Organization();
            expected.TotalBeds = 10;

            // Create the test flow.
            var testFlow = await CreateTestFlow(CapacityDialog.Name);

            // Execute the conversation.
            await testFlow
                .Test("begin", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Send(expected.TotalBeds.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoHousing()
        {
            // Create the test flow.
            var testFlow = await CreateTestFlow(CapacityDialog.Name);

            // Execute the conversation.
            await testFlow
                .Test("begin", StartsWith(Phrases.Capacity.GetHasHousing))
                .Send("no")
                .StartTestAsync();
        }
    }
}
