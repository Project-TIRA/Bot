using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Capacity
{
    public class HousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Valid()
        {
            var expected = new Organization();
            expected.TotalBeds = 10;

            // Create the test flow.
            var testFlow = await CreateTestFlow(HousingDialog.Name);

            // Execute the conversation.
            await testFlow
                .Test("begin", Phrases.Capacity.GetHousingTotal)
                .Send(expected.TotalBeds.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task Invalid()
        {
            // Create the test flow.
            var testFlow = await CreateTestFlow(HousingDialog.Name);

            // Execute the conversation.
            await testFlow
                .Test("begin", Phrases.Capacity.GetHousingTotal)
                .Test("5", Phrases.Capacity.GetHousingOpen)
                .Test("10", Phrases.Capacity.GetHousingError)
                .StartTestAsync();
        }
    }
}
