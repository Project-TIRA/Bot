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
    public class ServiceTypeDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NoType()
        {
            await CreateTestFlow(ServiceTypeDialog.Name)
                .Test("test", StartsWith(Phrases.Search.GetServiceType))
                .Send(Phrases.Services.Housing.ServiceName)
                .StartTestAsync();

            var expectedContext = new ConversationContext();
            expectedContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);

            // Validate the results.
            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            Assert.Equal(expectedContext, actualContext);
        }       
    }
}
