using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace SearchProviderBotTests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateEmploymentDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreatEmploymentData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateEmploymentDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Send(ServiceProviderBotTestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<EmploymentData>(this.turnContext, organization.Id, true);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.JobReadinessTrainingOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.PaidInternshipOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.VocationalTrainingOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.EmploymentPlacementOpen);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreatEmploymentData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateEmploymentDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Employment.JobReadinessTraining)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Employment.PaidInternships)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Employment.VocationalTraining)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Employment.EmploymentPlacement)))
                .Send(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<EmploymentData>(this.turnContext, organization.Id, true);
            Assert.Equal(0, resultData.JobReadinessTrainingOpen);
            Assert.Equal(0, resultData.PaidInternshipOpen);
            Assert.Equal(0, resultData.VocationalTrainingOpen);
            Assert.Equal(0, resultData.EmploymentPlacementOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.JobReadinessTrainingWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.PaidInternshipWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.VocationalTrainingWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.EmploymentPlacementWaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreatEmploymentData(this.api, user.Id, service.Id, true, false, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateEmploymentDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<EmploymentData>(this.turnContext, organization.Id, true);
            Assert.Equal(0, resultData.JobReadinessTrainingOpen);
            Assert.Equal(0, resultData.PaidInternshipOpen);
            Assert.Equal(0, resultData.VocationalTrainingOpen);
            Assert.Equal(0, resultData.EmploymentPlacementOpen);
            Assert.False(resultData.JobReadinessTrainingWaitlistIsOpen);
            Assert.False(resultData.PaidInternshipWaitlistIsOpen);
            Assert.False(resultData.VocationalTrainingWaitlistIsOpen);
            Assert.False(resultData.EmploymentPlacementWaitlistIsOpen);
        }
    }
}
