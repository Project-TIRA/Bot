using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace SearchProviderBotTests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCaseManagementDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateCaseManagementData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateCaseManagementDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Send(ServiceProviderBotTestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(this.turnContext, true);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.Open);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateCaseManagementData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateCaseManagementDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.CaseManagement.ServiceName)))
                .Send(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(this.turnContext, true);
            Assert.Equal(0, resultData.Open);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.WaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateCaseManagementData(this.api, user.Id, service.Id, true, false, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateCaseManagementDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(this.turnContext, true);
            Assert.Equal(0, resultData.Open);
            Assert.False(resultData.WaitlistIsOpen);
        }
    }
}
