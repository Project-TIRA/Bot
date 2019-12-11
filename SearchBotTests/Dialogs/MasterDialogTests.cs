using System;
using System.Collections.Generic;
using System.Linq;
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
        public void UniqueLuisEntities()
        {
            IDictionary<string, ServiceData> entities = new Dictionary<string, ServiceData>();

            foreach (var dataType in Helpers.GetServiceDataTypes())
            {
                foreach (var luisEntity in dataType.LuisEntityNames())
                {
                    // Make sure each LUIS entity only has a single data type to handle it.
                    Assert.DoesNotContain(luisEntity, entities);
                    entities.Add(luisEntity, dataType);
                }
            }
        }

        [Fact]
        public async Task UnknownIntent()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("test", SearchBot.Phrases.Search.GetServiceType)
                .StartTestAsync();
        }

        /*
        [Theory]
        [MemberData(nameof(TestTypes))]
        public async Task GetServices(ServiceData dataType)
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

        [Theory]
        [MemberData(nameof(TestTypePairs))]
        public async Task GetMultipleServices(ServiceData dataType1, ServiceData dataType2)
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
        */
    }
}
