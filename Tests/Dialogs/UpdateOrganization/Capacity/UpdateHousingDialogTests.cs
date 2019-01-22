using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using TestBot.Bot.Dialogs.UpdateOrganization.Capacity;
using TestBot.Bot.Models;
using TestBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialogTests : DialogTestBase
    {
        /*
        [Fact]
        public async Task Valid()
        {
            var expected = new OrganizationProfile();
            expected.Capacity.Beds.Total = 10;
            expected.Capacity.Beds.Open = 5;

            // Set an initial profile.
            var initialProfile = new OrganizationProfile();
            initialProfile.Capacity.Beds.Total = expected.Capacity.Beds.Total;

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test(expected.Capacity.Beds.Total.ToString(), Phrases.Capacity.GetHousingOpen)
                .Send(expected.Capacity.Beds.Open.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }
        */

        [Fact]
        public async Task Invalid()
        {
            // Set an initial profile to trigger updates.
            var initialProfile = new OrganizationProfile();
            initialProfile.Capacity.Beds.Total = 0;

            var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(initialProfile.Capacity.Beds.Total));

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test("5", error)
                .StartTestAsync();
        }
    }
}
