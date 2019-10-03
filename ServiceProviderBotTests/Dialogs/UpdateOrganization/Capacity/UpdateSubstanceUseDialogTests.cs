using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace SearchProviderBotTests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateSubstanceUseDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateSubstanceUseData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateSubstanceUseDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Send(ServiceProviderBotTestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<SubstanceUseData>(organization.Id, this.turnContext);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.DetoxOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.InPatientOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.OutPatientOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.GroupOpen);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateSubstanceUseData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateSubstanceUseDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.SubstanceUse.Detox)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.SubstanceUse.InPatient)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.SubstanceUse.OutPatient)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.SubstanceUse.Group)))
                .Send(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<SubstanceUseData>(organization.Id, this.turnContext);
            Assert.Equal(0, resultData.DetoxOpen);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.Equal(0, resultData.GroupOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.DetoxWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.InPatientWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.OutPatientWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.GroupWaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateSubstanceUseData(this.api, user.Id, service.Id, true, false, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateSubstanceUseDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<SubstanceUseData>(organization.Id, this.turnContext);
            Assert.Equal(0, resultData.DetoxOpen);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.Equal(0, resultData.GroupOpen);
            Assert.False(resultData.DetoxWaitlistIsOpen);
            Assert.False(resultData.InPatientWaitlistIsOpen);
            Assert.False(resultData.OutPatientWaitlistIsOpen);
            Assert.False(resultData.GroupWaitlistIsOpen);
        }
    }
}
