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
                .Test("test", Phrases.Capacity.CaseManagement.GetOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.Housing.GetEmergencySharedBedsOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.Housing.GetLongTermSharedBedsOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.JobTraining.GetOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.MentalHealth.GetInPatientOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.MentalHealth.GetOutPatientOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.SubstanceUse.GetDetoxOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.SubstanceUse.GetInPatientOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.SubstanceUse.GetOutPatientOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.SubstanceUse.GetGroupOpen)
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();
        }
    }
}
