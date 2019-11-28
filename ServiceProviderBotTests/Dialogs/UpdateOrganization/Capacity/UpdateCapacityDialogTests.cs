using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace SearchProviderBotTests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCapacityDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var caseManagementService = await TestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var housingService = await TestHelpers.CreateService<HousingData>(this.api, organization.Id);
            var employmentService = await TestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var mentalHealthService = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var substanceUseService = await TestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);

            var caseManagementData = await TestHelpers.CreateCaseManagementData(this.api, user.Id, caseManagementService.Id);   
            var housingData = await TestHelpers.CreateHousingData(this.api, user.Id, housingService.Id);
            var employmentData = await TestHelpers.CreatEmploymentData(this.api, user.Id, employmentService.Id);
            var mentalHealthData = await TestHelpers.CreateMentalHealthData(this.api, user.Id, mentalHealthService.Id);
            var substanceUseData = await TestHelpers.CreateSubstanceUseData(this.api, user.Id, substanceUseService.Id);

            await CreateTestFlow(UpdateCapacityDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencySharedBeds))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencyPrivateBeds))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermSharedBeds))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermPrivateBeds))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();
        }
    }
}
