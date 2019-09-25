using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs;
using Shared;
using Xunit;

namespace SearchProviderBotTests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task InvalidChannel()
        {
            await CreateTestFlow(MasterDialog.Name, user: null, channelOverride: Channels.Webchat)
                .Send("test")
                .StartTestAsync();

            // Can't access turnContext before the first turn, so must split the message and response apart.
            await CreateTestFlow(MasterDialog.Name, user: null)
                .AssertReply(Phrases.Greeting.InvalidChannel(this.turnContext))
                .StartTestAsync();
        }

        [Fact]
        public async Task EmulatorChannel()
        {
            User user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organizationId: string.Empty);

            await CreateTestFlow(MasterDialog.Name, user, channelOverride: Channels.Emulator)
                .Test("test", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task SmsChannel()
        {
            User user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organizationId: string.Empty);

            await CreateTestFlow(MasterDialog.Name, user, channelOverride: Channels.Sms)
                .Test("test", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task NotRegistered()
        {
            await CreateTestFlow(MasterDialog.Name, user: null)
                .Send("test")
                .StartTestAsync();

            // Can't access turnContext before the first turn, so must split the message and response apart.
            await CreateTestFlow(MasterDialog.Name, user: null)
                .AssertReply(Phrases.Greeting.NotRegistered(this.turnContext))
                .StartTestAsync();
        }

        [Fact]
        public async Task NoOrganization()
        {
            User user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organizationId: string.Empty);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("test", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task OrganizationNotVerified()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: false);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("test", Phrases.Greeting.UnverifiedOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task Enable()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test(Phrases.Keywords.Enable, Phrases.Greeting.ContactEnabledUpdated(true))
                .StartTestAsync();

            user = await this.api.GetUser(this.turnContext);
            Assert.True(user.ContactEnabled);
        }

        [Fact]
        public async Task Disable()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            user.ContactEnabled = true;
            await this.api.Update(user);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test(Phrases.Keywords.Disable, Phrases.Greeting.ContactEnabledUpdated(false))
                .StartTestAsync();

            user = await this.api.GetUser(this.turnContext);
            Assert.True(!user.ContactEnabled);
        }

        [Fact]
        public async Task Reset()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateMentalHealthData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test(Phrases.Keywords.Update, Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test(ServiceProviderBotTestHelpers.DefaultTotal.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Test(Phrases.Keywords.Update, Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .StartTestAsync();
        }
    }
}
