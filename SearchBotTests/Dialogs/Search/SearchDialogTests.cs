﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityModel;
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
        [MemberData(nameof(TestTypes))]
        public async Task SingleService(ServiceData dataType)
        {
            var initialContext = new ConversationContext();
            initialContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);
            initialContext.CreateOrUpdateServiceContext(dataType, ServiceFlags.None);

            var testFlow = CreateTestFlow(SearchDialog.Name, initialContext)
                .Send("test");

            if (dataType.ServiceCategories().Count != 0)
            {
                testFlow = testFlow
                    .AssertReply(StartsWith(SearchBot.Phrases.Search.GetSpecificType(dataType)))
                    .Send(dataType.ServiceCategories().First().Name);
            }

            await testFlow.StartTestAsync();
        }

        [Theory]
        [MemberData(nameof(TestTypes))]
        public async Task NoLocation(ServiceData dataType)
        {
            var initialContext = new ConversationContext();
            initialContext.CreateOrUpdateServiceContext(dataType, ServiceFlags.None);

            await CreateTestFlow(SearchDialog.Name, initialContext)
                .Test("test", SearchBot.Phrases.Search.GetLocation)
                .StartTestAsync();
        }

        [Theory]
        [MemberData(nameof(TestTypePairs))]
        public async Task MultipleServices(ServiceData dataType1, ServiceData dataType2)
        {
            var initialContext = new ConversationContext();
            initialContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);
            initialContext.CreateOrUpdateServiceContext(dataType1, dataType1.ServiceCategories().Count != 0 ? dataType1.ServiceCategories().First().ServiceFlags() : ServiceFlags.None);
            initialContext.CreateOrUpdateServiceContext(dataType2, dataType2.ServiceCategories().Count != 0 ? dataType2.ServiceCategories().First().ServiceFlags() : ServiceFlags.None);

            var testFlow = CreateTestFlow(SearchDialog.Name, initialContext)
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

        [Theory]
        [MemberData(nameof(TestTypePairs))]
        public async Task MultipleServicesNoLocation(ServiceData dataType1, ServiceData dataType2)
        {
            var initialContext = new ConversationContext();
            initialContext.CreateOrUpdateServiceContext(dataType1, dataType1.ServiceCategories().Count != 0 ? dataType1.ServiceCategories().First().ServiceFlags() : ServiceFlags.None);
            initialContext.CreateOrUpdateServiceContext(dataType2, dataType2.ServiceCategories().Count != 0 ? dataType2.ServiceCategories().First().ServiceFlags() : ServiceFlags.None);

            await CreateTestFlow(SearchDialog.Name, initialContext)
                .Test("test", SearchBot.Phrases.Search.GetLocation)
                .StartTestAsync();
        }
    }
}