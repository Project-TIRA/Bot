using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using SearchBot.Bot.Dialogs.Service;
using SearchBot.Bot.State;
using Shared;
using Shared.Models;
using Xunit;

namespace SearchBotTests.Dialogs
{
    public class RecommendationTests : DialogTestBase
    {
        [Fact]
        public async Task SingleServiceFullMatch()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);

            var organizationFlags = ServiceFlags.HousingEmergency;
            var requestedFlags = ServiceFlags.HousingEmergency;
            var match = CreateMatchData(organization, organizationFlags, requestedFlags);

            await RunTest(match);
        }

        [Fact]
        public async Task SingleServiceNoMatch()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);

            var organizationFlags = ServiceFlags.HousingEmergency;
            var requestedFlags = ServiceFlags.Employment;
            var match = CreateMatchData(organization, organizationFlags, requestedFlags);

            await RunTest(match, expectedMatch: false);
        }

        [Fact]
        public async Task MultipleServicesFullMatch()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);

            var organizationFlags = ServiceFlags.CaseManagement | ServiceFlags.MentalHealth;
            var requestedFlags = ServiceFlags.CaseManagement | ServiceFlags.MentalHealth;
            var match = CreateMatchData(organization, organizationFlags, requestedFlags);

            await RunTest(match);
        }

        [Fact]
        public async Task MultipleServicesComboMatch()
        {
            var organization1 = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var organization2 = await TestHelpers.CreateOrganization(this.api, isVerified: true);

            var organization1Flags = ServiceFlags.SubstanceUseDetox;
            var organization2Flags = ServiceFlags.EmploymentInternship;
            var requestedFlags = ServiceFlags.SubstanceUseDetox | ServiceFlags.EmploymentInternship;

            var match1 = CreateMatchData(organization1, organization1Flags, requestedFlags);
            var match2 = CreateMatchData(organization2, organization2Flags, requestedFlags);

            await RunTest(new List<MatchData>() { match1, match2 });
        }

        [Fact]
        public async Task MultipleServicesNoMatch()
        {
            var organization1 = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var organization2 = await TestHelpers.CreateOrganization(this.api, isVerified: true);

            var organization1Flags = ServiceFlags.HousingLongTerm;
            var organization2Flags = ServiceFlags.SubstanceUse;
            var requestedFlags = ServiceFlags.HousingEmergency | ServiceFlags.MentalHealth;

            var match1 = CreateMatchData(organization1, organization1Flags, requestedFlags);
            var match2 = CreateMatchData(organization2, organization2Flags, requestedFlags);

            await RunTest(new List<MatchData>() { match1, match2 }, expectedMatch: false);
        }

        private MatchData CreateMatchData(Organization organization, ServiceFlags organizationFlags, ServiceFlags requestedFlags)
        {
            MatchData match = new MatchData();
            match.Organization = organization;
            match.OrganizationServiceFlags = organizationFlags;
            match.RequestedServiceFlags = requestedFlags;

            // Set the service types based on the flags.
            foreach (var flag in match.OrganizationServiceFlags.GetFlags())
            {
                var serviceType = flag.ToServiceType();
                if (!match.OrganizationServiceTypes.Contains(serviceType))
                {
                    match.OrganizationServiceTypes.Add(serviceType);
                }
            }

            return match;
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
                // Create the organization's services.
                foreach (var serviceType in match.OrganizationServiceTypes)
                {
                    var service = await TestHelpers.CreateService(this.api, match.Organization.Id, serviceType);
                    await TestHelpers.CreateServiceData(this.api, service);
                }

                // Update the conversation context with the requested service types.
                foreach (var flag in match.RequestedServiceFlags.GetFlags())
                {
                    conversationContext.CreateOrUpdateServiceContext(flag.ToServiceType(), flag);
                }
            }

            var expectedRecommendation = expectedMatch ?
                SearchBot.Phrases.Search.MakeRecommendation(matches) :
                SearchBot.Phrases.Search.NoMatch(conversationContext);

            await CreateTestFlow(ServiceDialog.Name, conversationContext)
                .Test("test", expectedRecommendation)
                .StartTestAsync();
        }
    }
}
