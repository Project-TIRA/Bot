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
    public class ServiceDialogTests : DialogTestBase
    {
        [Fact]
        public async Task SingleService()
        {
            var initialContext = new ConversationContext();
            initialContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);
            initialContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);

            await CreateTestFlow(ServiceDialog.Name, initialContext)
                .Test("test", StartsWith(Phrases.Search.GetHousingType))
                .StartTestAsync();
        }

        [Fact]
        public async Task NoLocation()
        {
            var initialContext = new ConversationContext();
            initialContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);

            await CreateTestFlow(ServiceDialog.Name, initialContext)
                .Test("test", Phrases.Search.GetLocation)
                .StartTestAsync();
        }

        [Fact]
        public async Task MultipleServices()
        {
            var initialContext = new ConversationContext();
            initialContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);
            initialContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);
            initialContext.CreateOrUpdateServiceContext(ServiceType.Employment, ServiceFlags.Employment);

            await CreateTestFlow(ServiceDialog.Name, initialContext)
                .Test("test", StartsWith(Phrases.Search.GetHousingType))
                .StartTestAsync();
        }

        [Fact]
        public async Task MultipleServicesNoLocation()
        {
            var initialContext = new ConversationContext();
            initialContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.None);
            initialContext.CreateOrUpdateServiceContext(ServiceType.Employment, ServiceFlags.Employment);

            await CreateTestFlow(ServiceDialog.Name, initialContext)
                .Test("test", Phrases.Search.GetLocation)
                .StartTestAsync();
        }
    }
}
