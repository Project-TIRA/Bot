using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using SearchBot.Bot.Dialogs.Search;
using SearchBot.Bot.State;
using Xunit;

namespace SearchBotTests.Dialogs.Search
{
    public class ServicesDialogTests : DialogTestBase
    {
        [Theory]
        [MemberData(nameof(TestTypes))]
        public async Task SubServiceCategory(ServiceData dataType)
        {
            if (dataType.SubServiceCategories().Count > 0)
            {
                var category = dataType.SubServiceCategories().First();

                var expectedContext = new ConversationContext();
                expectedContext.CreateOrUpdateServiceContext(dataType, ServiceFlags.None);

                await CreateTestFlow(ServicesDialog.Name, expectedContext)
                    .Test("test", StartsWith(SearchBot.Phrases.Search.GetSpecificType(dataType)))
                    .Send(category.Name)
                    .StartTestAsync();

                expectedContext.CreateOrUpdateServiceContext(dataType, category.ServiceFlag);

                // Validate the results.
                var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
                Assert.Equal(expectedContext, actualContext);
            }
        }       
    }
}
