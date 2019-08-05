
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs;
using Shared;
using Shared.ApiInterface;
using Xunit;

namespace Tests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NotRegistered()
        {
            await CreateTestFlow(MasterDialog.Name, user: null)
                .Test("hi", Phrases.Greeting.NotRegistered)
                .StartTestAsync();
        }

        [Fact]
        public async Task NoOrganization()
        {
            User user = await CreateUser(organizationId: string.Empty);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("hi", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task OrganizationNotVerified()
        {
            var organization = await CreateOrganization(isVerified: false);
            var user = await CreateUser(organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("hi", Phrases.Greeting.UnverifiedOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task Help()
        {
            var organization = await CreateOrganization(isVerified: true);
            var user = await CreateUser(organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send(Phrases.Greeting.HelpKeyword)
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Greeting.Help)
                .StartTestAsync();
        }

        [Fact]
        public async Task NothingToUpdate()
        {
            var organization = await CreateOrganization(isVerified: true);
            var user = await CreateUser(organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send("update")
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Update.NothingToUpdate)
                .StartTestAsync();
        }

        /*
        [Fact]
        public async Task Update()
        {
            var organization = await CreateOrganization(isVerified: true);
            var user = await CreateUser(organization.Id);
            var service = await CreateService(organization.Id, ServiceType.Housing);
            var housingData = await CreateHousingData(service.Id, true, 10, 10, 10, 10);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send("update")
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Capacity.Housing.GetEmergencySharedBedsOpen)
                .Test("5", Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen)
                .Test("5", Phrases.Capacity.Housing.GetLongTermSharedBedsOpen)
                .Test("5", Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen)
                .Test("5", Phrases.Update.Closing)
                .StartTestAsync();
        }
        */
    }
}
