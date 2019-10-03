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
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var caseManagementService = await ServiceProviderBotTestHelpers.CreateService<CaseManagementData>(this.api, organization.Id);
            var housingService = await ServiceProviderBotTestHelpers.CreateService<HousingData>(this.api, organization.Id);
            var employmentService = await ServiceProviderBotTestHelpers.CreateService<EmploymentData>(this.api, organization.Id);
            var mentalHealthService = await ServiceProviderBotTestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var substanceUseService = await ServiceProviderBotTestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);

            var caseManagementData = await ServiceProviderBotTestHelpers.CreateCaseManagementData(this.api, user.Id, caseManagementService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal);   
            var housingData = await ServiceProviderBotTestHelpers.CreateHousingData(this.api, user.Id, housingService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);
            var employmentData = await ServiceProviderBotTestHelpers.CreatEmploymentData(this.api, user.Id, employmentService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);
            var mentalHealthData = await ServiceProviderBotTestHelpers.CreateMentalHealthData(this.api, user.Id, mentalHealthService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);
            var substanceUseData = await ServiceProviderBotTestHelpers.CreateSubstanceUseData(this.api, user.Id, substanceUseService.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateCapacityDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.CaseManagement.ServiceName))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.JobReadinessTraining))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.PaidInternships))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.VocationalTraining))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Employment.EmploymentPlacement))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencySharedBeds))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencyPrivateBeds))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermSharedBeds))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermPrivateBeds))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Send(ServiceProviderBotTestHelpers.DefaultOpen.ToString())
                .StartTestAsync();
        }
    }
}
