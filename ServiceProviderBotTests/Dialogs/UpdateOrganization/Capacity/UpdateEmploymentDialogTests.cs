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
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data = await TestHelpers.CreatEmploymentData(this.api, user.Id, service.Id);

            await CreateTestFlow(UpdateEmploymentDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<EmploymentData>(organization.Id, this.turnContext);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.JobReadinessTrainingOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.PaidInternshipOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.VocationalTrainingOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.EmploymentPlacementOpen);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data = await TestHelpers.CreatEmploymentData(this.api, user.Id, service.Id);

            await CreateTestFlow(UpdateEmploymentDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Employment.JobReadinessTraining)))
                .Test(TestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Employment.PaidInternships)))
                .Test(TestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Employment.VocationalTraining)))
                .Test(TestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Employment.EmploymentPlacement)))
                .Send(TestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<EmploymentData>(organization.Id, this.turnContext);
            Assert.Equal(0, resultData.JobReadinessTrainingOpen);
            Assert.Equal(0, resultData.PaidInternshipOpen);
            Assert.Equal(0, resultData.VocationalTrainingOpen);
            Assert.Equal(0, resultData.EmploymentPlacementOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.JobReadinessTrainingWaitlistIsOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.PaidInternshipWaitlistIsOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.VocationalTrainingWaitlistIsOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.EmploymentPlacementWaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var data = await TestHelpers.CreatEmploymentData(this.api, user.Id, service.Id, hasWaitlist: false);

            await CreateTestFlow(UpdateEmploymentDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<EmploymentData>(organization.Id, this.turnContext);
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
