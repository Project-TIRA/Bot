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
            var data = await TestHelpers.CreateSubstanceUseData(this.api, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateSubstanceUseDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<SubstanceUseData>(this.userToken, true);
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
            var data = await TestHelpers.CreateSubstanceUseData(this.api, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateSubstanceUseDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Services.SubstanceUse.Detox))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Services.SubstanceUse.InPatient))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Services.SubstanceUse.OutPatient))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Services.SubstanceUse.Group))
                .Send(TestHelpers.DefaultWaitlistLength.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<SubstanceUseData>(this.userToken, true);
            Assert.Equal(0, resultData.DetoxOpen);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.Equal(0, resultData.GroupOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.DetoxWaitlistLength);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.InPatientWaitlistLength);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.OutPatientWaitlistLength);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.GroupWaitlistLength);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<SubstanceUseData>(this.api, organization.Id);
            var data = await TestHelpers.CreateSubstanceUseData(this.api, service.Id, true, false, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateSubstanceUseDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Detox))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.InPatient))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.OutPatient))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.SubstanceUse.Group))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<SubstanceUseData>(this.userToken, true);
            Assert.Equal(0, resultData.DetoxOpen);
            Assert.Equal(0, resultData.InPatientOpen);
            Assert.Equal(0, resultData.OutPatientOpen);
            Assert.Equal(0, resultData.GroupOpen);
        }
    }
}
