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
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await TestHelpers.CreateMentalHealthData(this.api, user.Id, service.Id);

            await CreateTestFlow(UpdateMentalHealthDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<MentalHealthData>(organization.Id, this.turnContext);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.InPatientOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.OutPatientOpen);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await TestHelpers.CreateMentalHealthData(this.api, user.Id, service.Id);

            await CreateTestFlow(UpdateMentalHealthDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.MentalHealth.InPatient)))
                .Test(TestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.MentalHealth.OutPatient)))
                .Send(TestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<MentalHealthData>(organization.Id, this.turnContext);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.InPatientWaitlistIsOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.OutPatientWaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<MentalHealthData>(this.api, organization.Id);
            var data = await TestHelpers.CreateMentalHealthData(this.api, user.Id, service.Id, hasWaitlist: false);

            await CreateTestFlow(UpdateMentalHealthDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.InPatient))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.MentalHealth.OutPatient))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<MentalHealthData>(organization.Id, this.turnContext);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.False(resultData.InPatientWaitlistIsOpen);
            Assert.False(resultData.OutPatientWaitlistIsOpen);
        }
    }
}
