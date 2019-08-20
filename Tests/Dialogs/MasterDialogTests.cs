
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
                .Test("test", Phrases.Greeting.NotRegistered)
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
                .Test(Phrases.Keywords.Enable, Phrases.Greeting.ContactEnabledUpdated(true))
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
                .Test(Phrases.Keywords.Disable, Phrases.Greeting.ContactEnabledUpdated(false))
                .StartTestAsync();

            user = await this.api.GetUser(this.userToken);
            Assert.True(!user.ContactEnabled);
        }

        [Fact]
        public async Task Reset()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await TestHelpers.CreateMentalHealthData(this.api, user.Id, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test(Phrases.Keywords.Update, Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test(TestHelpers.DefaultTotal.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Test(Phrases.Keywords.Update, Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .StartTestAsync();
        }
    }
}
