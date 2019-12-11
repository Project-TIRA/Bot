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
        public async Task ClarifyServiceCategory(ServiceData dataType)
        {
            foreach (var serviceCategory in dataType.ServiceCategories())
            {
                foreach (var subService in serviceCategory.Services)
                {
                    var expectedContext = new ConversationContext();
                    expectedContext.CreateOrUpdateServiceContext(dataType, ServiceFlags.None);

                    await CreateTestFlow(ServicesDialog.Name, expectedContext)
                        .Test("test", StartsWith(SearchBot.Phrases.Search.GetSpecificType(dataType)))
                        .Send(serviceCategory.Name)
                        .StartTestAsync();

                    expectedContext.CreateOrUpdateServiceContext(dataType, serviceCategory.ServiceFlags());

                    // Validate the results.
                    var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
                    Assert.Equal(expectedContext, actualContext);
                }
            }
        }       
    }
}
