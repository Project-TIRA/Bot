using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using TestBot.Bot.Dialogs.UpdateOrganization.Capacity;
using TestBot.Bot.Models;
using TestBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCapacityDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UpdateAll()
        {
            var expected = new OrganizationProfile();
            expected.Capacity.Beds.Total = 10;
            expected.Capacity.Beds.Open = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateCapacityDialog.Name)
                .Test("begin", Phrases.Capacity.GetHousingTotal)
                .Test(expected.Capacity.Beds.Total.ToString(), Phrases.Capacity.GetHousingOpen)
                .Send(expected.Capacity.Beds.Open.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NoHousing()
        {
            // Execute the conversation.
            await CreateTestFlow(UpdateCapacityDialog.Name)
                .Send("begin")
                .StartTestAsync();
        }
    }
}
