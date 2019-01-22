using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Location;
using ServiceProviderBot.Bot.Models.OrganizationProfile;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Location
{
    public class LocationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Valid()
        {
            var expected = new OrganizationProfile();
            expected.Location.City = TestOrgCity;
            expected.Location.State = TestOrgState;
            expected.Location.Zip = TestOrgZip;

            // Execute the conversation.
            await CreateTestFlow(LocationDialog.Name)
                .Test("begin", Phrases.Location.GetLocation)
                .Send(expected.Location.Zip)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task Invalid()
        {
            // Execute the conversation.
            await CreateTestFlow(LocationDialog.Name)
                .Test("begin", Phrases.Location.GetLocation)
                .Test("0000000000", Phrases.Location.GetLocation)
                .StartTestAsync();
        }

        [Fact]
        public async Task NotFound()
        {
            // Execute the conversation.
            await CreateTestFlow(LocationDialog.Name)
                .Test("begin", Phrases.Location.GetLocation)
                .Test("12345", Phrases.Location.GetLocationError)
                .StartTestAsync();
        }
    }
}
