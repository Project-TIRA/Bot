﻿using EntityModel.Helpers;
using SearchBot.Bot.Dialogs.Search;
using SearchBot.Bot.State;
using Shared;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SearchBotTests.Dialogs.Search
{
    public class RecommendationDialogTests : DialogTestBase
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

        [Theory]
        [MemberData(nameof(TestServiceFlags))]
        public async Task DistanceMid(ServiceFlags serviceFlag)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            organization.Latitude = TestHelpers.DefaultLocationPositionMid.Lat.ToString();
            organization.Longitude = TestHelpers.DefaultLocationPositionMid.Lon.ToString();

            var match = new MatchData() { Organization = organization, OrganizationServiceFlags = serviceFlag, RequestedServiceFlags = serviceFlag };
            await RunTest(match, expectedMatch: true, expectedDistance: ConversationContext.SEARCH_DISTANCE_MID);
        }

        [Theory]
        [MemberData(nameof(TestServiceFlags))]
        public async Task DistanceLong(ServiceFlags serviceFlag)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            organization.Latitude = TestHelpers.DefaultLocationPositionLong.Lat.ToString();
            organization.Longitude = TestHelpers.DefaultLocationPositionLong.Lon.ToString();

            var match = new MatchData() { Organization = organization, OrganizationServiceFlags = serviceFlag, RequestedServiceFlags = serviceFlag };
            await RunTest(match, expectedMatch: true, expectedDistance: ConversationContext.SEARCH_DISTANCE_LONG);
        }

        [Theory]
        [MemberData(nameof(TestServiceFlags))]
        public async Task DistanceTooFar(ServiceFlags serviceFlag)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            organization.Latitude = TestHelpers.DefaultLocationPositionTooFar.Lat.ToString();
            organization.Longitude = TestHelpers.DefaultLocationPositionTooFar.Lon.ToString();

            var match = new MatchData() { Organization = organization, OrganizationServiceFlags = serviceFlag, RequestedServiceFlags = serviceFlag };
            await RunTest(match, expectedMatch: false, expectedDistance: ConversationContext.SEARCH_DISTANCE_LONG);
        }

        private async Task RunTest(MatchData matches, bool expectedMatch = true, int expectedDistance = ConversationContext.SEARCH_DISTANCE_SHORT)
        {
            await RunTest(new List<MatchData>() { matches }, expectedMatch, expectedDistance);
        }

        private async Task RunTest(List<MatchData> matches, bool expectedMatch = true, int expectedDistance = ConversationContext.SEARCH_DISTANCE_SHORT)
        {
            var conversationContext = new ConversationContext();
            conversationContext.TEST_SetLocation(TestHelpers.DefaultLocation, TestHelpers.DefaultLocationPosition);

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

            var testFlow = CreateTestFlow(RecommendationDialog.Name, conversationContext)
                .Send("test");

            // Start the test to initialize the turn context.
            await testFlow.StartTestAsync();

            var actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);

            // Keep searching wider until no longer possible or the expected distance is reached.
            while (actualContext.CanExpandSearchDistance() && expectedDistance > actualContext.SearchDistance)
            {
                testFlow = testFlow.AssertReply(StartsWith(SearchBot.Phrases.Search.NoMatchSearchWider(actualContext)));
                testFlow = testFlow.Send("yes");
                await testFlow.StartTestAsync();

                actualContext = await this.state.GetConversationContext(this.turnContext, this.cancellationToken);
            }

            if (expectedMatch)
            {
                testFlow = testFlow.AssertReply(SearchBot.Phrases.Search.MakeRecommendation(matches));
            }
            else
            {
                if (actualContext.CanExpandSearchDistance())
                {
                    testFlow = testFlow.AssertReply(StartsWith(SearchBot.Phrases.Search.NoMatchSearchWider(actualContext)));
                }
                else
                {
                    testFlow = testFlow.AssertReply(SearchBot.Phrases.Search.NoMatch(actualContext));
                }
            }

            await testFlow.StartTestAsync();
        }
    }
}
