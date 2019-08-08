using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
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
            var jobTrainingService = await TestHelpers.CreateService<JobTrainingData>(this.api, organization.Id);
            var mentalHealthService = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var substanceUseService = await TestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);

            var caseManagementData = await TestHelpers.CreateCaseManagementData(this.api, caseManagementService.Id, true, true, TestHelpers.DefaultTotal);   
            var housingData = await TestHelpers.CreateHousingData(this.api, housingService.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);
            var jobTrainingData = await TestHelpers.CreateJobTrainingData(this.api, jobTrainingService.Id, true, true, TestHelpers.DefaultTotal);
            var mentalHealthData = await TestHelpers.CreateMentalHealthData(this.api, mentalHealthService.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);
            var substanceUseData = await TestHelpers.CreateSubstanceUseData(this.api, substanceUseService.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateCapacityDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.Name))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencySharedBeds))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencyPrivateBeds))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermSharedBeds))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermPrivateBeds))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.JobTraining.Name))
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
