using System.Threading.Tasks;
using EntityModel;
using SearchBot.Bot.Dialogs.Search;
using SearchBot.Bot.State;
using Xunit;

namespace SearchBotTests.Dialogs.Search
{
    public class ServiceTypeDialogTests : DialogTestBase
    {
        [Theory]
        [MemberData(nameof(TestTypes))]
        public async Task NoType(ServiceData dataType)
        {
            foreach (var serviceCategory in dataType.ServiceCategories())
            {
                foreach (var subService in serviceCategory.Services)
                {
                    await CreateTestFlow(ServiceTypeDialog.Name)
                        .Test("test", StartsWith(SearchBot.Phrases.Search.GetServiceType))
                        .Send(subService.Name)
                        .StartTestAsync();

                    var expectedContext = new ConversationContext();
                    expectedContext.CreateOrUpdateServiceContext(dataType, subService.ServiceFlags);

                    // Validate the results.
                    var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
                    Assert.Equal(expectedContext, actualContext);
                }
            }
        }       
    }
}
