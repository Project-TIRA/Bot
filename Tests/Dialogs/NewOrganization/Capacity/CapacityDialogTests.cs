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

            // Execute the conversation.
            await CreateTestFlow(CapacityDialog.Name)
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
            // Execute the conversation.
            await CreateTestFlow(CapacityDialog.Name)
                .Test("begin", StartsWith(Phrases.Capacity.GetHasHousing))
                .Send("no")
                .StartTestAsync();
        }
    }
}
