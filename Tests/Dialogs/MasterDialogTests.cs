
using System;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs;
using Shared;
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
            User user = await TestHelpers.CreateUser(this.api, organizationId: string.Empty);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("hi", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task OrganizationNotVerified()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: false);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("hi", Phrases.Greeting.UnverifiedOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task Enable()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send(Phrases.Greeting.EnableKeyword)
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Greeting.ContactEnabledUpdated(true))
                .StartTestAsync();

            user = await this.api.GetUser(this.userToken);
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
                .Send(Phrases.Greeting.DisableKeyword)
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Greeting.ContactEnabledUpdated(false))
                .StartTestAsync();

            user = await this.api.GetUser(this.userToken);
            Assert.True(!user.ContactEnabled);
        }

        [Fact]
        public async Task Expired()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await TestHelpers.CreateCaseManagementData(this.api, user.Id, service.Id, false, true, TestHelpers.DefaultTotal);

            data.CreatedOn = DateTime.UtcNow.AddHours(-Phrases.Reset.TimeoutHours);
            await this.api.Update(data);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send("hi")
                .Test(Phrases.Reset.Keyword, Phrases.Reset.Expired(user))
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(this.userToken, true);
            Assert.Null(resultData);
        }

        [Fact]
        public async Task ForceReset()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Greeting.Keywords(user.ContactEnabled))
                .Test(Phrases.Reset.Keyword, Phrases.Reset.Forced(user))
                .StartTestAsync();
        }
    }
}
