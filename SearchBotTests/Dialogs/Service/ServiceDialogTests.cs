using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs.Service;
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
            initialContext.Location = SearchBotTestHelpers.DefaultLocation;
            initialContext.Housing = true;

            await CreateTestFlow(ServiceDialog.Name, initialContext)
                .Test("test", StartsWith(Phrases.Search.GetHousingType))
                .StartTestAsync();
        }

        [Fact]
        public async Task NoLocation()
        {
            var initialContext = new ConversationContext();
            initialContext.Housing = true;

            await CreateTestFlow(ServiceDialog.Name, initialContext)
                .Test("test", Phrases.Search.GetLocation)
                .StartTestAsync();
        }

        [Fact]
        public async Task MultipleServices()
        {
            var initialContext = new ConversationContext();
            initialContext.Location = SearchBotTestHelpers.DefaultLocation;
            initialContext.Housing = true;
            initialContext.Employment = true;

            await CreateTestFlow(ServiceDialog.Name, initialContext)
                .Test("test", StartsWith(Phrases.Search.GetHousingType))
                .StartTestAsync();
        }

        [Fact]
        public async Task MultipleServicesNoLocation()
        {
            var initialContext = new ConversationContext();
            initialContext.Housing = true;
            initialContext.Employment = true;

            await CreateTestFlow(ServiceDialog.Name, initialContext)
                .Test("test", Phrases.Search.GetLocation)
                .StartTestAsync();
        }
    }
}
