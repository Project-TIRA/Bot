using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using EntityModel.Luis;
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
                foreach (var luisMapping in dataType.LuisMappings())
                {
                    // Make sure each LUIS entity only has a single mapping.
                    Assert.DoesNotContain(luisMapping.EntityName, entities);
                    entities.Add(luisMapping.EntityName, dataType);
                }
            }
        }

        [Fact]
        public void UniqueServiceFlags()
        {
            ServiceFlags flags = ServiceFlags.None;

            foreach (var dataType in Helpers.GetServiceDataTypes())
            {
                foreach (var serviceCategory in dataType.ServiceCategories())
                {
                    // Make sure each service flag only has a single service category to handle it.
                    Assert.True((flags & serviceCategory.ServiceFlags) == 0);
                    flags |= serviceCategory.ServiceFlags;
                }
            }
        }

        [Fact]
        public void AllLuisEntities()
        {
            // Make sure each LUIS entity is handled.
            var entities = typeof(LuisModel).GetFields().Where(f => f.FieldType == typeof(string[]));

            foreach (var entity in entities)
            {
                var (type, mapping) = ConversationContext.GetLuisMapping(entity.Name);
                Assert.NotNull(type);
                Assert.NotNull(mapping);
            }
        }

        [Fact]
        public void AllServiceFlags()
        {
            // Make sure each service flag is handled.
            foreach (var flag in Helpers.GetServiceFlags())
            {
                var match = Helpers.GetServiceDataTypes()
                    .Any(t => t.ServiceCategories()
                        .Any(c => c.Services
                            .Any(s => s.ServiceFlags.HasFlag(flag))));
                Assert.True(match);
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
