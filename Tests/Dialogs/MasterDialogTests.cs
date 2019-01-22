using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.Models.OrganizationProfile;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NewOrganization()
        {
            var expected = CreateDefaultTestProfile();

            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Test("begin", StartsWith(Phrases.Greeting.GetAction))
                .Test("new", Phrases.NewOrganization.GetName)
                .Test(expected.Name, Phrases.Location.GetLocation)
                .Test(expected.Location.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        /*
        [Fact]
        public async Task UpdateOrganization()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("hello", StartsWith(Phrases.Greeting.GetAction))
                .Test("update", "TODO")
                .StartTestAsync();
        }
        */
    }
}
