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
                .Test("test", Phrases.Capacity.SubstanceUse.GetDetoxOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.SubstanceUse.GetInPatientOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.SubstanceUse.GetOutPatientOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.SubstanceUse.GetGroupOpen)
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
                .Test("test", Phrases.Capacity.SubstanceUse.GetDetoxOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.SubstanceUse.DetoxServiceName))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.SubstanceUse.GetInPatientOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.SubstanceUse.InPatientServiceName))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.SubstanceUse.GetOutPatientOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.SubstanceUse.OutPatientServiceName))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.SubstanceUse.GetGroupOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.SubstanceUse.GroupServiceName))
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
                .Test("test", Phrases.Capacity.SubstanceUse.GetDetoxOpen)
                .Test("0", Phrases.Capacity.SubstanceUse.GetInPatientOpen)
                .Test("0", Phrases.Capacity.SubstanceUse.GetOutPatientOpen)
                .Test("0", Phrases.Capacity.SubstanceUse.GetGroupOpen)
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
