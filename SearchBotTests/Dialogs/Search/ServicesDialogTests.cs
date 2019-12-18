using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using EntityModel.Helpers;
using SearchBot.Bot.Dialogs.Search;
using SearchBot.Bot.State;
using Shared;
using Xunit;

namespace SearchBotTests.Dialogs.Search
{
    public class ServicesDialogTests : DialogTestBase
    {
        [Theory]
        [MemberData(nameof(TestDataTypes))]
        public async Task SingleServiceClarifyCategory(ServiceData dataType)
        {
            var initialContext = new ConversationContext();
            initialContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);
            initialContext.CreateOrUpdateServiceContext(dataType, dataType.ServiceCategories().Count == 1 ? dataType.LuisMappings().First().ServiceFlags : ServiceFlags.None);

            var testFlow = CreateTestFlow(ServicesDialog.Name, initialContext)
                .Send("test");

            if (dataType.ServiceCategories().Count > 1)
            {
                testFlow = testFlow
                    .AssertReply(StartsWith(SearchBot.Phrases.Search.GetSpecificType(dataType)))
                    .Send(dataType.ServiceCategories().First().Name);
            }

            await testFlow.StartTestAsync();
        }

        [Theory]
        [MemberData(nameof(TestDataTypePairs))]
        public async Task MultipleServicesClarifyCategory(ServiceData dataType1, ServiceData dataType2)
        {
            // Skip if the data types are the same.
            if (dataType1.ServiceType() != dataType2.ServiceType())
            {
                var initialContext = new ConversationContext();
                initialContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);
                initialContext.CreateOrUpdateServiceContext(dataType1, dataType1.ServiceCategories().Count == 1 ? dataType1.LuisMappings().First().ServiceFlags : ServiceFlags.None);
                initialContext.CreateOrUpdateServiceContext(dataType2, dataType2.ServiceCategories().Count == 1 ? dataType2.LuisMappings().First().ServiceFlags : ServiceFlags.None);

                var testFlow = CreateTestFlow(ServicesDialog.Name, initialContext)
                    .Send("test");

                if (dataType1.ServiceCategories().Count > 1)
                {
                    testFlow = testFlow
                        .AssertReply(StartsWith(SearchBot.Phrases.Search.GetSpecificType(dataType1)))
                        .Send(dataType1.ServiceCategories().First().Name);
                }

                if (dataType2.ServiceCategories().Count > 1)
                {
                    testFlow = testFlow
                        .AssertReply(StartsWith(SearchBot.Phrases.Search.GetSpecificType(dataType2)))
                        .Send(dataType2.ServiceCategories().First().Name);
                }

                await testFlow.StartTestAsync();
            }
        }
    }
}
