
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

            var service = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await TestHelpers.CreateMentalHealthData(this.api, user.Id, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            // Push back the time of the original snapshot so that it doesn't become the latest when the new snapshot time is pushed back.
            data.CreatedOn = DateTime.UtcNow.AddHours(-Phrases.Reset.TimeoutHours);
            await this.api.Update(data);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send(Phrases.Greeting.UpdateKeyword)
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test("3", Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .StartTestAsync();

            // Push back the time of the new snapshot.
            data = await this.api.GetLatestServiceData<MentalHealthData>(this.userToken, true);
            data.CreatedOn = DateTime.UtcNow.AddHours(-Phrases.Reset.TimeoutHours);
            await this.api.Update(data);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("4", Phrases.Reset.Expired(user))
                .Send(Phrases.Greeting.UpdateKeyword)
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .StartTestAsync();
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
