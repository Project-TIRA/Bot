using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace SearchProviderBotTests.Dialogs.UpdateOrganization
{
    public class UpdateOrganizationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NothingToUpdate()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(UpdateOrganizationDialog.Name, user)
                .Test("test", Phrases.Update.NothingToUpdate)
                .StartTestAsync();
        }

        [Fact]
        public async Task SingleService()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateCaseManagementData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateOrganizationDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .StartTestAsync();
        }

        [Fact]
        public async Task MultipleServicesUpdateAll()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service1 = await ServiceProviderBotTestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data1 = await ServiceProviderBotTestHelpers.CreateCaseManagementData(this.api, user.Id, service1.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal);

            var service2 = await ServiceProviderBotTestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data2 = await ServiceProviderBotTestHelpers.CreatEmploymentData(this.api, user.Id, service2.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateOrganizationDialog.Name, user)
                .Test("test", StartsWith(Phrases.Update.Options))
                .Test(Phrases.Services.All, Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Update.Closing)
                .StartTestAsync();
        }

        [Fact]
        public async Task MultipleServicesUpdateOne()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service1 = await ServiceProviderBotTestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var data1 = await ServiceProviderBotTestHelpers.CreateCaseManagementData(this.api, user.Id, service1.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal);

            var service2 = await ServiceProviderBotTestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data2 = await ServiceProviderBotTestHelpers.CreatEmploymentData(this.api, user.Id, service2.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateOrganizationDialog.Name, user)
                .Test("test", StartsWith(Phrases.Update.Options))
                .Test(Phrases.Services.Employment.ServiceName, Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Update.Closing)
                .StartTestAsync();
        }
    }
}
