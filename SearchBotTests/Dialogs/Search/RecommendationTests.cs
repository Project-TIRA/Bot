using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using EntityModel.Helpers;
using SearchBot.Bot.Dialogs.Search;
using SearchBot.Bot.State;
using Shared;
using Shared.Models;
using Xunit;

namespace SearchBotTests.Dialogs.Search
{
    public class RecommendationTests : DialogTestBase
    {
        [Theory]
        [MemberData(nameof(TestServiceFlags))]
        public async Task SingleServiceFullMatch(ServiceFlags serviceFlag)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var match = new MatchData() { Organization = organization, OrganizationServiceFlags = serviceFlag, RequestedServiceFlags = serviceFlag };
            await RunTest(match);
        }

        [Theory]
        [MemberData(nameof(TestServiceFlagPairs))]
        public async Task SingleServiceNoMatch(ServiceFlags serviceFlag1, ServiceFlags serviceFlag2)
        {
            // Skip if the flags can be handled by a single type.
            if (Helpers.ServiceFlagToDataType(serviceFlag1 | serviceFlag2) == null)
            {
                var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
                var match = new MatchData() { Organization = organization, OrganizationServiceFlags = serviceFlag1, RequestedServiceFlags = serviceFlag2 };
                await RunTest(match, expectedMatch: false);
            }
        }

        [Theory]
        [MemberData(nameof(TestServiceFlagPairs))]
        public async Task MultipleServicesFullMatch(ServiceFlags serviceFlag1, ServiceFlags serviceFlag2)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var match = new MatchData() { Organization = organization, OrganizationServiceFlags = serviceFlag1 | serviceFlag2, RequestedServiceFlags = serviceFlag1 | serviceFlag2 };
            await RunTest(match);
        }

        [Theory]
        [MemberData(nameof(TestServiceFlagPairs))]
        public async Task MultipleServicesComboMatch(ServiceFlags serviceFlag1, ServiceFlags serviceFlag2)
        {
            // Skip if the flags can be handled by a single type.
            if (Helpers.ServiceFlagToDataType(serviceFlag1 | serviceFlag2) == null)
            {
                var organization1 = await TestHelpers.CreateOrganization(this.api, isVerified: true);
                var organization2 = await TestHelpers.CreateOrganization(this.api, isVerified: true);

                var match1 = new MatchData() { Organization = organization1, OrganizationServiceFlags = serviceFlag1, RequestedServiceFlags = serviceFlag1 | serviceFlag2 };
                var match2 = new MatchData() { Organization = organization2, OrganizationServiceFlags = serviceFlag2, RequestedServiceFlags = serviceFlag1 | serviceFlag2 };
                await RunTest(new List<MatchData>() { match1, match2 });
            }
        }

        [Theory]
        [MemberData(nameof(TestServiceFlagPairs))]
        public async Task MultipleServicesNoMatch(ServiceFlags serviceFlag1, ServiceFlags serviceFlag2)
        {
            // Skip if the flags can be handled by a single type.
            if (Helpers.ServiceFlagToDataType(serviceFlag1 | serviceFlag2) == null)
            {
                var organization1 = await TestHelpers.CreateOrganization(this.api, isVerified: true);
                var organization2 = await TestHelpers.CreateOrganization(this.api, isVerified: true);

                var requestedFlags = serviceFlag1 | serviceFlag2;

                var organization1Flags = ServiceFlags.None;
                var organization2Flags = ServiceFlags.None;

                // Give the organizations a flag that does not match the requested ones.
                foreach (var flag in ServiceFlagsHelpers.AllFlags())
                {
                    if (!requestedFlags.HasFlag(flag))
                    {
                        organization1Flags = organization2Flags = flag;
                        break;
                    }
                }

                var match1 = new MatchData() { Organization = organization1, OrganizationServiceFlags = organization1Flags, RequestedServiceFlags = serviceFlag1 | serviceFlag2 };
                var match2 = new MatchData() { Organization = organization2, OrganizationServiceFlags = organization2Flags, RequestedServiceFlags = serviceFlag1 | serviceFlag2 };
                await RunTest(new List<MatchData>() { match1, match2 }, expectedMatch: false);
            }
        }

        private async Task RunTest(MatchData matches, bool expectedMatch = true)
        {
            await RunTest(new List<MatchData>() { matches }, expectedMatch);
        }

        private async Task RunTest(List<MatchData> matches, bool expectedMatch = true)
        {
            var conversationContext = new ConversationContext();
            conversationContext.TEST_SetLocation(SearchBotTestHelpers.DefaultLocation, SearchBotTestHelpers.DefaultLocationPosition);

            foreach (var match in matches)
            {
                // Create the organization's services and data based on the requested service flags.
                await TestHelpers.CreateServicesAndData(this.api, match.Organization.Id, string.Empty, false, match.OrganizationServiceFlags);
            }

            // Update the conversation context with the requested types.
            // Only do for the first match entry since they each have the same RequestedServiceFlags.
            if (matches.Count > 0)
            {
                foreach (var flag in ServiceFlagsHelpers.SplitFlags(matches[0].RequestedServiceFlags))
                {
                    var dataType = Helpers.ServiceFlagToDataType(flag);
                    conversationContext.CreateOrUpdateServiceContext(dataType, flag);
                }
            }

            var expectedRecommendation = expectedMatch ?
                SearchBot.Phrases.Search.MakeRecommendation(matches) :
                SearchBot.Phrases.Search.NoMatch(conversationContext);

            await CreateTestFlow(SearchDialog.Name, conversationContext)
                .Test("test", expectedRecommendation)
                .StartTestAsync();
        }
    }
}
