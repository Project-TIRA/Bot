using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs.Service;
using SearchBot.Bot.Models;
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
            expectedContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);

            await CreateTestFlow(HousingDialog.Name, expectedContext)
                .Test("test", StartsWith(Phrases.Search.GetHousingType))
                .Send(Phrases.Services.Housing.Emergency)
                .StartTestAsync();

            expectedContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.HousingEmergency);

            // Validate the results.
            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            Assert.Equal(expectedContext, actualContext);
        }       
    }
}
