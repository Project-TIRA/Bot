using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Demographic;
using ServiceProviderBot.Bot.Utils;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Demographic
{
    public class AgeRangeDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Valid()
        {
            var expected = new Organization();
            expected.AgeRangeStart = 14;
            expected.AgeRangeEnd = 24;

            // Create the test flow.
            var testFlow = await CreateTestFlow(AgeRangeDialog.Name);

            // Execute the conversation.
            await testFlow
                .Test("begin", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expected.AgeRangeStart.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Send(expected.AgeRangeEnd.ToString())
                .StartTestAsync();

            // Validate the profile.
            await ValidateProfile(expected);
        }

        [Fact]
        public async Task Invalid()
        {
            // Create the test flow.
            var testFlow = await CreateTestFlow(AgeRangeDialog.Name);

            // Execute the conversation.
            await testFlow
                .Test("begin", Phrases.AgeRange.GetAgeRangeStart)
                .Test("14", Phrases.AgeRange.GetAgeRangeEnd)
                .Test("10", Phrases.AgeRange.GetAgeRangeError)
                .StartTestAsync();
        }
    }
}
