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
        public async Task ClarifyType(ServiceData dataType)
        {
            foreach (var serviceCategory in dataType.ServiceCategories())
            {
                foreach (var subService in serviceCategory.Services)
                {
                    await CreateTestFlow(ServiceTypeDialog.Name)
                        .Test("test", StartsWith(SearchBot.Phrases.Search.GetServiceType))
                        .Send(subService.Name)
                        .StartTestAsync();

                    // Validate the results.
                    var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
                    Assert.True(subService.ServiceFlags.HasFlag(actualContext.RequestedServiceFlags()));
                }
            }
        }       
    }
}
