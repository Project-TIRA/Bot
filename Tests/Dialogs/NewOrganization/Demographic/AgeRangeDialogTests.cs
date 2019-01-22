using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using TestBot.Bot.Dialogs.NewOrganization.Demographic;
using TestBot.Bot.Models.OrganizationProfile;
using TestBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Demographic
{
    public class AgeRangeDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Valid()
        {
            var expected = new OrganizationProfile();
            expected.Demographic.AgeRange.Start = 14;
            expected.Demographic.AgeRange.End = 24;

            // Execute the conversation.
            await CreateTestFlow(AgeRangeDialog.Name)
                .Test("begin", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expected.Demographic.AgeRange.Start.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Send(expected.Demographic.AgeRange.End.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task Invalid()
        {
            // Execute the conversation.
            await CreateTestFlow(AgeRangeDialog.Name)
                .Test("begin", Phrases.AgeRange.GetAgeRangeStart)
                .Test("14", Phrases.AgeRange.GetAgeRangeEnd)
                .Test("10", Phrases.AgeRange.GetAgeRangeError)
                .StartTestAsync();
        }
    }
}
