using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateCapacityDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UpdateAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.HousingEmergencyPrivateTotal = 10;
            expectedOrganization.HousingEmergencySharedTotal = 8;
            expectedOrganization.HousingLongtermPrivateTotal = 6;
            expectedOrganization.HousingLongtermSharedTotal = 4;


            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.BedsEmergencyPrivateOpen = 5;
            expectedSnapshot.BedsEmergencySharedOpen = 4;
            expectedSnapshot.BedsLongtermPrivateOpen = 3;
            expectedSnapshot.BedsLongtermSharedOpen = 2;

            // Execute the conversation.
            await CreateTestFlow(UpdateCapacityDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.Capacity.GetHousingEmergencyPrivateOpen)
                .Test(expectedSnapshot.BedsEmergencyPrivateOpen.ToString(), Phrases.Capacity.GetHousingEmergencySharedOpen)
                .Test(expectedSnapshot.BedsEmergencySharedOpen.ToString(), Phrases.Capacity.GetHousingLongtermPrivateOpen)
                .Test(expectedSnapshot.BedsLongtermPrivateOpen.ToString(), Phrases.Capacity.GetHousingLongtermSharedOpen)
                .Send(expectedSnapshot.BedsLongtermSharedOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }
    }
}
