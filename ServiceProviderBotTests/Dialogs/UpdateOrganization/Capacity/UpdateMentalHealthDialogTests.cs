using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace SearchProviderBotTests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateMentalHealthDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateMentalHealthData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateMentalHealthDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Send(ServiceProviderBotTestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<MentalHealthData>(this.turnContext, true);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.InPatientOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.OutPatientOpen);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateMentalHealthData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateMentalHealthDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.MentalHealth.InPatient)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.MentalHealth.OutPatient)))
                .Send(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<MentalHealthData>(this.turnContext, true);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.InPatientWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.OutPatientWaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateMentalHealthData(this.api, user.Id, service.Id, true, false, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateMentalHealthDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<MentalHealthData>(this.turnContext, true);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.False(resultData.InPatientWaitlistIsOpen);
            Assert.False(resultData.OutPatientWaitlistIsOpen);
        }
    }
}
