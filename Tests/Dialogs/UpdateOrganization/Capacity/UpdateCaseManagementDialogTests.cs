using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCaseManagementDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await TestHelpers.CreateCaseManagementData(this.api, service.Id, true, true, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateCaseManagementDialog.Name, user)
                .Test("test", Phrases.Capacity.CaseManagement.GetOpen)
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(this.userToken, true);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.Open);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await TestHelpers.CreateCaseManagementData(this.api, service.Id, true, true, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateCaseManagementDialog.Name, user)
                .Test("test", Phrases.Capacity.CaseManagement.GetOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.CaseManagement.ServiceName))
                .Send(TestHelpers.DefaultWaitlistLength.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(this.userToken, true);
            Assert.Equal(0, resultData.Open);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.WaitlistLength);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await TestHelpers.CreateCaseManagementData(this.api, service.Id, true, false, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateCaseManagementDialog.Name, user)
                .Test("test", Phrases.Capacity.CaseManagement.GetOpen)
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<CaseManagementData>(this.userToken, true);
            Assert.Equal(0, resultData.Open);
            Assert.Equal(0, resultData.WaitlistLength);
        }
    }
}
