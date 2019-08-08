using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<HousingData>(this.api, organization.Id);
            var data = await TestHelpers.CreateHousingData(this.api, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateHousingDialog.Name, user)
                .Test("test", Phrases.Capacity.Housing.GetEmergencySharedBedsOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.Housing.GetLongTermSharedBedsOpen)
                .Test(TestHelpers.DefaultOpen.ToString(), Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen)
                .Send(TestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<HousingData>(this.userToken, true);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.EmergencySharedBedsOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.EmergencyPrivateBedsOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.LongTermSharedBedsOpen);
            Assert.Equal(TestHelpers.DefaultOpen, resultData.LongTermPrivateBedsOpen);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<HousingData>(this.api, organization.Id);
            var data = await TestHelpers.CreateHousingData(this.api, service.Id, true, true, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateHousingDialog.Name, user)
                .Test("test", Phrases.Capacity.Housing.GetEmergencySharedBedsOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.Housing.EmergencySharedBeds))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.Housing.EmergencyPrivateBeds))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.Housing.GetLongTermSharedBedsOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.Housing.LongTermSharedBeds))
                .Test(TestHelpers.DefaultWaitlistLength.ToString(), Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen)
                .Test("0", Phrases.Capacity.GetWaitlistLength(Phrases.Capacity.Housing.LongTermPrivateBeds))
                .Send(TestHelpers.DefaultWaitlistLength.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<HousingData>(this.userToken, true);
            Assert.Equal(0, resultData.EmergencySharedBedsOpen);
            Assert.Equal(0, resultData.EmergencyPrivateBedsOpen);
            Assert.Equal(0, resultData.LongTermSharedBedsOpen);
            Assert.Equal(0, resultData.LongTermPrivateBedsOpen);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.EmergencySharedBedsWaitlistLength);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.EmergencyPrivateBedsWaitlistLength);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.LongTermSharedBedsWaitlistLength);
            Assert.Equal(TestHelpers.DefaultWaitlistLength, resultData.LongTermPrivateBedsWaitlistLength);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var service = await TestHelpers.CreateService<HousingData>(this.api, organization.Id);
            var data = await TestHelpers.CreateHousingData(this.api, service.Id, true, false, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal, TestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateHousingDialog.Name, user)
                .Test("test", Phrases.Capacity.Housing.GetEmergencySharedBedsOpen)
                .Test("0", Phrases.Capacity.Housing.GetEmergencyPrivateBedsOpen)
                .Test("0", Phrases.Capacity.Housing.GetLongTermSharedBedsOpen)
                .Test("0", Phrases.Capacity.Housing.GetLongTermPrivateBedsOpen)
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<HousingData>(this.userToken, true);
            Assert.Equal(0, resultData.EmergencySharedBedsOpen);
            Assert.Equal(0, resultData.EmergencyPrivateBedsOpen);
            Assert.Equal(0, resultData.LongTermSharedBedsOpen);
            Assert.Equal(0, resultData.LongTermPrivateBedsOpen);
        }
    }
}
