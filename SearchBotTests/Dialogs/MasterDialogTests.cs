using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs;
using SearchBot.Bot.State;
using Shared;
using Xunit;

namespace SearchBotTests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UnknownIntent()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("test", Phrases.Intents.Unknown)
                .StartTestAsync();
        }

        [Fact]
        public async Task GetServices()
        {
            var initialContext = new ConversationContext();
            initialContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);
            initialContext.Housing = true;

            await CreateTestFlow(MasterDialog.Name)
                .Send($"where can I find {Phrases.Services.Housing.ServiceName} in {SearchBotTestHelpers.DefaultLocation}")
                .StartTestAsync();

            // Validate the results.
            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            Assert.Equal(initialContext, actualContext);
        }

        [Fact]
        public async Task GetMultipleServices()
        {
            var initialContext = new ConversationContext();
            initialContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);
            initialContext.Housing = true;
            initialContext.Employment = true;

            await CreateTestFlow(MasterDialog.Name)
                .Send($"where can I find {Phrases.Services.Housing.ServiceName} and {Phrases.Services.Employment.ServiceName} in {SearchBotTestHelpers.DefaultLocation}")
                .StartTestAsync();

            // Validate the results.
            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            Assert.Equal(initialContext, actualContext);
        }
    }
}
