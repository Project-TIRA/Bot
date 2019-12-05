using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using SearchBot;
using SearchBot.Bot.Dialogs;
using SearchBot.Bot.State;
using Shared;
using Shared.Models;
using Xunit;

namespace SearchBotTests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UnknownIntent()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("test", SearchBot.Phrases.Search.GetServiceType)
                .StartTestAsync();
        }

        [Fact]
        public async Task GetServices()
        {
            var initialContext = new ConversationContext();
            initialContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);
            initialContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);

            await CreateTestFlow(MasterDialog.Name)
                .Send($"where can I find {HousingData.SERVICE_NAME} in {SearchBotTestHelpers.DefaultLocation}")
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
            initialContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);
            initialContext.CreateOrUpdateServiceContext(ServiceType.Employment, ServiceFlags.Employment);

            await CreateTestFlow(MasterDialog.Name)
                .Send($"where can I find {HousingData.SERVICE_NAME} and {EmploymentData.SERVICE_NAME} in {SearchBotTestHelpers.DefaultLocation}")
                .StartTestAsync();

            // Validate the results.
            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            Assert.Equal(initialContext, actualContext);
        }
    }
}
