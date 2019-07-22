using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Capacity
{
    public class FrequencyDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Daily()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.UpdateFrequency = Frequency.Daily;

            // Execute the conversation.
            await CreateTestFlow(FrequencyDialog.Name, expectedOrganization)
                .Test("begin", StartsWith(Phrases.Capacity.GetFrequency))
                .Send(expectedOrganization.UpdateFrequency.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task Weekly()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.UpdateFrequency = Frequency.Weekly;

            // Execute the conversation.
            await CreateTestFlow(FrequencyDialog.Name, expectedOrganization)
                .Test("begin", StartsWith(Phrases.Capacity.GetFrequency))
                .Send(expectedOrganization.UpdateFrequency.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task Invalid()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.UpdateFrequency = Frequency.Daily;

            // Execute the conversation.
            await CreateTestFlow(FrequencyDialog.Name)
                .Test("begin", StartsWith(Phrases.Capacity.GetFrequency))
                .Test("never", StartsWith(Phrases.Capacity.GetFrequency))
                .StartTestAsync();
        }
    }
}
