using EntityModel;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace SearchProviderBotTests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Update()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<HousingData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateHousingData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateHousingDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencySharedBeds))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencyPrivateBeds))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermSharedBeds))
                .Test(ServiceProviderBotTestHelpers.DefaultOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermPrivateBeds))
                .Send(ServiceProviderBotTestHelpers.DefaultOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<HousingData>(organization.Id, this.turnContext);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.EmergencySharedBedsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.EmergencyPrivateBedsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.LongTermSharedBedsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultOpen, resultData.LongTermPrivateBedsOpen);
        }

        [Fact]
        public async Task Waitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<HousingData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateHousingData(this.api, user.Id, service.Id, true, true, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateHousingDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencySharedBeds))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Housing.EmergencySharedBeds)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencyPrivateBeds))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Housing.EmergencyPrivateBeds)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermSharedBeds))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Housing.LongTermSharedBeds)))
                .Test(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString(), Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermPrivateBeds))
                .Test("0", StartsWith(Phrases.Capacity.GetWaitlistIsOpen(Phrases.Services.Housing.LongTermPrivateBeds)))
                .Send(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<HousingData>(organization.Id, this.turnContext);
            Assert.Equal(0, resultData.EmergencySharedBedsOpen);
            Assert.Equal(0, resultData.EmergencyPrivateBedsOpen);
            Assert.Equal(0, resultData.LongTermSharedBedsOpen);
            Assert.Equal(0, resultData.LongTermPrivateBedsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.EmergencySharedBedsWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.LongTermPrivateBedsWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.LongTermSharedBedsWaitlistIsOpen);
            Assert.Equal(ServiceProviderBotTestHelpers.DefaultWaitlistIsOpen, resultData.LongTermPrivateBedsWaitlistIsOpen);
        }

        [Fact]
        public async Task NoWaitlist()
        {
            var organization = await ServiceProviderBotTestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await ServiceProviderBotTestHelpers.CreateUser(this.api, organization.Id);

            var service = await ServiceProviderBotTestHelpers.CreateService<HousingData>(this.api, organization.Id);
            var data = await ServiceProviderBotTestHelpers.CreateHousingData(this.api, user.Id, service.Id, true, false, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal, ServiceProviderBotTestHelpers.DefaultTotal);

            await CreateTestFlow(UpdateHousingDialog.Name, user)
                .Test("test", Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencySharedBeds))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.Housing.EmergencyPrivateBeds))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermSharedBeds))
                .Test("0", Phrases.Capacity.GetOpenings(Phrases.Services.Housing.LongTermPrivateBeds))
                .Send("0")
                .StartTestAsync();

            // Validate the results.
            var resultData = await this.api.GetLatestServiceData<HousingData>(organization.Id, this.turnContext);
            Assert.Equal(0, resultData.EmergencySharedBedsOpen);
            Assert.Equal(0, resultData.EmergencyPrivateBedsOpen);
            Assert.Equal(0, resultData.LongTermSharedBedsOpen);
            Assert.Equal(0, resultData.LongTermPrivateBedsOpen);
            Assert.False(resultData.EmergencySharedBedsWaitlistIsOpen);
            Assert.False(resultData.EmergencyPrivateBedsWaitlistIsOpen);
            Assert.False(resultData.LongTermSharedBedsWaitlistIsOpen);
            Assert.False(resultData.LongTermPrivateBedsWaitlistIsOpen);
        }
    }
}
