
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
            User user = CreateUser(organizationId: string.Empty);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("hi", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task OrganizationNotVerified()
        {
            var organization = await CreateOrganization(isVerified: false);
            var user = CreateUser(organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("hi", Phrases.Greeting.UnverifiedOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task Help()
        {
            var organization = await CreateOrganization(isVerified: true);
            var user = CreateUser(organization.Id);

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
            var user = CreateUser(organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send("update")
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Update.NothingToUpdate)
                .StartTestAsync();
        }

        [Fact]
        public async Task Update()
        {
            var organization = await CreateOrganization(isVerified: true);
            var user = CreateUser(organization.Id);
            var service = await CreateService(organization.Id, ServiceType.Housing);
            var housingData = await CreateHousingData(service.Id, true, 10, 10, 10, 10);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send("update")
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Capacity.Housing.GetEmergencySharedBedsOpen)
                .Test("5", Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen)
                .Test("5", Phrases.Capacity.Housing.GetLongTermSharedBedsOpen)
                .Test("5", Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen)
                .AssertReply(Phrases.Update.Closing)
                .StartTestAsync();
        }

        [Fact]
        public async Task UpdateOrganizationNoWaitlists()
        {

        }

        [Fact]
        public async Task NonKeywordUpdateOrganization()
        {
            /*
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.IsVerified = true;

            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name, initialOrganization)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Registered)
                .AssertReply(Phrases.Greeting.Keywords)
                .Test(Phrases.Greeting.Update, Phrases.UpdateOrganization.NothingToUpdate)
                .StartTestAsync();
            */
        }

        [Fact]
        public async Task NonKeywordInvalid()
        {
            /*
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .AssertReply(Phrases.Greeting.GetHelp)
                .Test("hi", Phrases.Greeting.GetHelp)
                .StartTestAsync();
            */
        }

        [Fact]
        public async Task Reset()
        {
            /*
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .AssertReply(Phrases.Greeting.GetHelp)
                .Send("reset")
                .AssertReply(Phrases.Greeting.GetHelp)
                .StartTestAsync();
            */
        }
    }
}
