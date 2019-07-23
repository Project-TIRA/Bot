using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.CaseManagement;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.CaseManagement
{
    public class UpdateCaseManagementDialogTests : DialogTestBase
    {
        [Fact]
        public async Task UpdateOpenSpace()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.CaseManagementTotal = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.CaseManagementOpenSlots = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateCaseManagementDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.CaseManagement.GetCaseManagementOpen)
                .Send(expectedSnapshot.CaseManagementOpenSlots.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task UpdateOpenSpaceError()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.CaseManagementTotal = 20;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.CaseManagementOpenSlots = 15;

            // Execute the conversation.
            await CreateTestFlow(UpdateCaseManagementDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.CaseManagement.GetCaseManagementOpen)
                .Test("25", string.Format(Phrases.CaseManagement.GetCaseManagementSpaceErrorFormat(expectedOrganization.CaseManagementTotal)))
                .Send(expectedSnapshot.CaseManagementOpenSlots.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task UpdateOpenSpaceWithWaitlistAvailable()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.CaseManagementTotal = 10;
            expectedOrganization.CaseManagementHasWaitlist = true;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            // Since there are still some spaces open, the dialog should not prompt for waitlist length
            expectedSnapshot.CaseManagementOpenSlots = 5;
            expectedSnapshot.CaseManagementWaitlistLength = 0;

            // Execute the conversation.
            await CreateTestFlow(UpdateCaseManagementDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.CaseManagement.GetCaseManagementOpen)
                .Send(expectedSnapshot.CaseManagementOpenSlots.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task UpdateNoOpenSpaceWithWaitlistAvailable()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.CaseManagementTotal = 10;
            expectedOrganization.CaseManagementHasWaitlist = true;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            // Since there are no spaces open, the dialog should prompt for waitlist length
            expectedSnapshot.CaseManagementOpenSlots = 0;
            expectedSnapshot.CaseManagementWaitlistLength = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateCaseManagementDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.CaseManagement.GetCaseManagementOpen)
                .Test(expectedSnapshot.CaseManagementOpenSlots.ToString(), Phrases.CaseManagement.GetCaseManagementWaitlistLength)
                .Send(expectedSnapshot.CaseManagementWaitlistLength.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }
    }
}
