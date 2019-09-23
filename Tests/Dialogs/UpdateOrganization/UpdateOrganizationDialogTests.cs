using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization
{
    public class UpdateOrganizationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NothingToUpdate()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(UpdateOrganizationDialog.Name, user)
                .Test("test", Phrases.Update.NothingToUpdate)
                .StartTestAsync();
        }

        [Fact]
        public async Task SingleService()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await TestHelpers.CreateCaseManagementData(this.api, user.Id, service.Id, true, true, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateOrganizationDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .StartTestAsync();
        }

        [Fact]
        public async Task MultipleServicesUpdateAll()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service1 = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data1 = await TestHelpers.CreateCaseManagementData(this.api, user.Id, service1.Id, true, true, TestHelpers.DefaultTotal);

            var service2 = await TestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data2 = await TestHelpers.CreatEmploymentData(this.api, user.Id, service2.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateOrganizationDialog.Name, user)
                .Test("test", StartsWith(Phrases.Update.Options))
                .Test(Phrases.Services.All, Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Update.Closing)
                .StartTestAsync();
        }

        [Fact]
        public async Task MultipleServicesUpdateOne()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service1 = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data1 = await TestHelpers.CreateCaseManagementData(this.api, user.Id, service1.Id, true, true, TestHelpers.DefaultTotal);

            var service2 = await TestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data2 = await TestHelpers.CreatEmploymentData(this.api, user.Id, service2.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateOrganizationDialog.Name, user)
                .Test("test", StartsWith(Phrases.Update.Options))
                .Test(Phrases.Services.Employment.ServiceName, Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Update.Closing)
                .StartTestAsync();
        }
    }
}
