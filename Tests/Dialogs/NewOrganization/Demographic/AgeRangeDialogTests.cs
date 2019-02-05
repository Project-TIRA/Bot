using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Demographic;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Demographic
{
    public class AgeRangeDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Valid()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.AgeRangeStart = 14;
            expectedOrganization.AgeRangeEnd = 24;

            // Execute the conversation.
            await CreateTestFlow(AgeRangeDialog.Name, expectedOrganization)
                .Test("begin", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expectedOrganization.AgeRangeStart.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Send(expectedOrganization.AgeRangeEnd.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task Invalid()
        {
            var initialOrganization = CreateDefaultTestOrganization();

            // Execute the conversation.
            await CreateTestFlow(AgeRangeDialog.Name, initialOrganization)
                .Test("begin", Phrases.AgeRange.GetAgeRangeStart)
                .Test("14", Phrases.AgeRange.GetAgeRangeEnd)
                .Test("10", Phrases.AgeRange.GetAgeRangeError)
                .StartTestAsync();
        }
    }
}
