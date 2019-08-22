using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using Shared.ApiInterface;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateSubstanceUseDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);
            var data = await TestHelpers.CreateSubstanceUseData(this.api, user.Id, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateSubstanceUseDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<SubstanceUseData>(this.turnContext, true);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.DetoxOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.InPatientOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.OutPatientOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.GroupOpen);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);
            var data = await TestHelpers.CreateSubstanceUseData(this.api, user.Id, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateSubstanceUseDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.SubstanceUse.Detox)))
                .Test(TestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.SubstanceUse.InPatient)))
                .Test(TestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.SubstanceUse.OutPatient)))
                .Test(TestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.SubstanceUse.Group)))
                .Send(TestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<SubstanceUseData>(this.turnContext, true);
            Assert.Equal(0, resultData.DetoxOpen);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.Equal(0, resultData.GroupOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.DetoxWaitlistIsOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.InPatientWaitlistIsOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.OutPatientWaitlistIsOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistIsOpen, resultData.GroupWaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);
            var data = await TestHelpers.CreateSubstanceUseData(this.api, user.Id, service.Id, true, false, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateSubstanceUseDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<SubstanceUseData>(this.turnContext, true);
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
