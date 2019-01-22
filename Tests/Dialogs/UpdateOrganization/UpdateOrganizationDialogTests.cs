using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using ServiceProviderBot.Bot.Models.OrganizationProfile;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization
{
    public class UpdateOrganizationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UpdateAll()
        {
            var expected = new OrganizationProfile();
            expected.Capacity.Beds.Total = 10;
            expected.Capacity.Beds.Open = 5;

            // Set an initial profile to trigger updates.
            var initialProfile = new OrganizationProfile();
            initialProfile.Capacity.Beds.Total = expected.Capacity.Beds.Total;

            // Execute the conversation.
            await CreateTestFlow(UpdateOrganizationDialog.Name, initialProfile)
                .Test("begin", Phrases.Capacity.GetHousingOpen)
                .Test(expected.Capacity.Beds.Open.ToString(), Phrases.UpdateOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task NothingToUpdate()
        {
            var expected = new OrganizationProfile();

            // Execute the conversation.
            await CreateTestFlow(UpdateOrganizationDialog.Name)
                .Test("begin", Phrases.UpdateOrganization.NothingToUpdate)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }
    }
}
