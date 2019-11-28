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
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await TestHelpers.CreateCaseManagementData(this.api, user.Id, service.Id);

            await CreateTestFlow(UpdateCaseManagementDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(organization.Id, this.turnContext);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.Open);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await TestHelpers.CreateCaseManagementData(this.api, user.Id, service.Id);

            await CreateTestFlow(UpdateCaseManagementDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.CaseManagement.ServiceName)))
                .Send(TestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(organization.Id, this.turnContext);
            Assert.Equal(0, resultData.Open);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.WaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await TestHelpers.CreateCaseManagementData(this.api, user.Id, service.Id, hasWaitlist: false);

            await CreateTestFlow(UpdateCaseManagementDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(organization.Id, this.turnContext);
            Assert.Equal(0, resultData.Open);
            Assert.False(resultData.WaitlistIsOpen);
        }
    }
}
