using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs.Service;
using SearchBot.Bot.State;
using Shared.Models;
using Xunit;

namespace SearchBotTests.Dialogs
{
    public class HousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NoType()
        {
            var expectedContext = new ConversationContext();
            expectedContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);

            await CreateTestFlow(HousingDialog.Name, expectedContext)
                .Test("test", StartsWith(SearchBot.Phrases.Search.GetHousingType))
                .Send(Shared.Phrases.Services.Housing.Emergency)
                .StartTestAsync();

            expectedContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.HousingEmergency);

            // Validate the results.
            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            Assert.Equal(expectedContext, actualContext);
        }       
    }
}
