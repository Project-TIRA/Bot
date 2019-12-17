using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs;
using Shared;
using Xunit;

namespace ServiceProviderBotTests.Dialogs
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
            User user = await TestHelpers.CreateUser(this.api, organizationId: string.Empty);

            await CreateTestFlow(MasterDialog.Name, user, channelOverride: Channels.Emulator)
                .Test("test", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task SmsChannel()
        {
            User user = await TestHelpers.CreateUser(this.api, organizationId: string.Empty);

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
            User user = await TestHelpers.CreateUser(this.api, organizationId: string.Empty);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("test", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task OrganizationNotVerified()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: false);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("test", Phrases.Greeting.UnverifiedOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task Enable()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test(Phrases.Keywords.Enable, Phrases.Preferences.ContactEnabledUpdated(true))
                .StartTestAsync();

            user = await this.api.GetUser(this.turnContext);
            Assert.True(user.ContactEnabled);
        }

        [Fact]
        public async Task Disable()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            user.ContactEnabled = true;
            await this.api.Update(user);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test(Phrases.Keywords.Disable, Phrases.Preferences.ContactEnabledUpdated(false))
                .StartTestAsync();

            user = await this.api.GetUser(this.turnContext);
            Assert.True(!user.ContactEnabled);
        }

        [Fact]
        public async Task Feedback()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test(Phrases.Keywords.Feedback, Phrases.Feedback.GetFeedback)
                .Test(Phrases.Keywords.Feedback, Phrases.Feedback.Thanks)
                .StartTestAsync();
        }

        [Theory]
        [MemberData(nameof(TestTypes))]
        public async Task Reset(ServiceData dataType)
        {
            if (dataType.ServiceCategories()[0].Services.Count > 1)
            {
                var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
                var user = await TestHelpers.CreateUser(this.api, organization.Id);

                var service = await TestHelpers.CreateService(this.api, organization.Id, dataType.ServiceType());
                var data = await TestHelpers.CreateServiceData(this.api, user.Id, service.Id, dataType);

                var services = dataType.ServiceCategories()[0].Services;

                await CreateTestFlow(MasterDialog.Name, user)
                    .Test(Phrases.Keywords.Update, Phrases.Capacity.GetOpenings(services[0].Name))
                    .Test(TestHelpers.DefaultTotal.ToString(), Phrases.Capacity.GetOpenings(services[1].Name))
                    .Test(Phrases.Keywords.Update, Phrases.Capacity.GetOpenings(services[0].Name))
                    .StartTestAsync();
            }
        }
    }
}
