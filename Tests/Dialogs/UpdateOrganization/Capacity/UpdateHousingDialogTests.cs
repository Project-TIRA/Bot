using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialogTests : DialogTestBase
    {
        [Fact]
        public async Task EmergencyPrivateHasWaitlist()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.HousingEmergencyPrivateTotal = 10;
            expectedOrganization.HousingHasWaitlist = true;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.BedsEmergencyPrivateOpen = 0;
            expectedSnapshot.BedsEmergencyPrivateWaitlistLength = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingEmergencyPrivateOpen)
                .Test(expectedSnapshot.BedsEmergencyPrivateOpen.ToString(), StartsWith(Phrases.Capacity.GetHousingEmergencyPrivateWaitlist))
                .Send(expectedSnapshot.BedsEmergencyPrivateWaitlistLength.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task EmergencyPrivateNoWaitlist()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.HousingEmergencyPrivateTotal = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.BedsEmergencyPrivateOpen = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingEmergencyPrivateOpen)
                .Send(expectedSnapshot.BedsEmergencyPrivateOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task UpdateAllNoWaitlist()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.HousingEmergencyPrivateTotal = 10;
            expectedOrganization.HousingEmergencySharedTotal = 10;
            expectedOrganization.HousingLongtermPrivateTotal = 10;
            expectedOrganization.HousingLongtermSharedTotal = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.BedsEmergencyPrivateOpen = 8;
            expectedSnapshot.BedsEmergencySharedOpen = 7;
            expectedSnapshot.BedsLongtermPrivateOpen = 6;
            expectedSnapshot.BedsLongtermSharedOpen = 5;

            await CreateTestFlow(UpdateHousingDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingEmergencyPrivateOpen)
                .Test(expectedSnapshot.BedsEmergencyPrivateOpen.ToString(), Phrases.Capacity.GetHousingEmergencySharedOpen)
                .Test(expectedSnapshot.BedsEmergencySharedOpen.ToString(), Phrases.Capacity.GetHousingLongtermPrivateOpen)
                .Test(expectedSnapshot.BedsLongtermPrivateOpen.ToString(), Phrases.Capacity.GetHousingLongtermSharedOpen)
                .Send(expectedSnapshot.BedsLongtermSharedOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task EmergencyPrivateInvalid()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.HousingEmergencyPrivateTotal = 10;

            var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(expectedOrganization.HousingEmergencyPrivateTotal));

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.BedsEmergencyPrivateOpen = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingEmergencyPrivateOpen)
                .Test("20", error)
                .Send(expectedSnapshot.BedsEmergencyPrivateOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task LongtermPrivateInvalid()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.HousingLongtermPrivateTotal = 5;

            var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(expectedOrganization.HousingLongtermPrivateTotal));

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.BedsLongtermPrivateOpen = 3;

            // Execute the conversation.
            await CreateTestFlow(UpdateHousingDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingLongtermPrivateOpen)
                .Test("20", error)
                .Send(expectedSnapshot.BedsLongtermPrivateOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }
    }
}
