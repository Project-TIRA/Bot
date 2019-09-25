using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs.Service;
using SearchBot.Bot.State;
using Shared;
using Xunit;

namespace SearchBotTests.Dialogs
{
    public class HousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NoType()
        {
            var expectedContext = new ConversationContext();
            expectedContext.Housing = true;

            await CreateTestFlow(HousingDialog.Name, expectedContext)
                .Test("test", StartsWith(Phrases.Search.GetHousingType))
                .Send(Phrases.Services.Housing.Emergency)
                .StartTestAsync();

            expectedContext.HousingEmergency = true;

            // Validate the results.
            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            Assert.Equal(expectedContext, actualContext);
        }       
    }
}
