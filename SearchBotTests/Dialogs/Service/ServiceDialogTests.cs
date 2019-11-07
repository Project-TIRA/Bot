using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs.Service;
using SearchBot.Bot.State;
using Shared;
using System.Threading.Tasks;
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
                .Test("test", Phrases.Search.GetLocation(initialContext.GetServicesString()))
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
                .Test("test", Phrases.Search.GetLocation(initialContext.GetServicesString()))
                .StartTestAsync();
        }

        [Fact]
        public async Task MultipleServicesRecomendation()
        {
            var Services = Phrases.Services.Housing.ServiceName + " " + Phrases.Services.Employment.ServiceName;
            await CreateTestFlow(ServiceTypeDialog.Name)
              .Test("test", StartsWith(Phrases.Search.GetServiceType))
               //.Test(Services, StartsWith(Phrases.Search.GetLocation(Services)))
               //.Test(System.Enum.GetName(typeof(SearchBotTestHelpers.Location), 1), StartsWith(Phrases.Search.GetHousingType))
               //.Test("1", StartsWith(Phrases.Services.Responsestart))
               .StartTestAsync();
        }
    }
}