using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using TestBot.Bot.Dialogs.UpdateOrganization.Capacity;
using TestBot.Bot.Models.OrganizationProfile;
using TestBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialogTests : DialogTestBase
    {
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
            await CreateTestFlow(UpdateHousingDialog.Name, initialProfile)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Send(expected.Capacity.Beds.Open.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task Invalid()
        {
            var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(0));

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test("5", error)
                .StartTestAsync();
        }
    }
}
