using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using EntityModel.Helpers;
using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs.Search;
using SearchBot.Bot.State;
using Shared;
using Xunit;

namespace SearchBotTests.Dialogs.Search
{
    public class SearchDialogTests : DialogTestBase
    {
        [Theory]
        [MemberData(nameof(TestDataTypes))]
        public async Task SingleServiceNoLocation(ServiceData dataType)
        {
            var initialContext = new ConversationContext();
            initialContext.CreateOrUpdateServiceContext(dataType, ServiceFlags.None);

            await CreateTestFlow(SearchDialog.Name, initialContext)
                .Test("test", SearchBot.Phrases.Search.GetLocation)
                .StartTestAsync();
        }

        [Theory]
        [MemberData(nameof(TestDataTypePairs))]
        public async Task MultipleServicesNoLocation(ServiceData dataType1, ServiceData dataType2)
        {
            var initialContext = new ConversationContext();
            initialContext.CreateOrUpdateServiceContext(dataType1, ServiceFlags.None);
            initialContext.CreateOrUpdateServiceContext(dataType2, ServiceFlags.None);

            await CreateTestFlow(SearchDialog.Name, initialContext)
                .Test("test", SearchBot.Phrases.Search.GetLocation)
                .StartTestAsync();
        }
    }
}
