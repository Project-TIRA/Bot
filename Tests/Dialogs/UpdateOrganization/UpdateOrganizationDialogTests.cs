using System;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using Shared;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization
{
    public class UpdateOrganizationDialogTests : DialogTestBase
    {
        //[Fact]
        //public async Task UpdateAll()
        //{
        //    var expectedOrganization = CreateDefaultTestOrganization();
        //    expectedOrganization.BedsTotal = 10;
            expectedOrganization.HasJobTrainingServices = true;
            expectedOrganization.TotalJobTrainingPositions = 10;
            expectedOrganization.HasJobTrainingWaitlist = true;
            expectedOrganization.CaseManagementTotal = 10;

        //    var expectedSnapshot = new Snapshot(expectedOrganization.Id);
        //    expectedSnapshot.BedsOpen = 5;
            expectedSnapshot.OpenJobTrainingPositions = 5;
            expectedSnapshot.JobTrainingWaitlistPositions = 2;
            expectedSnapshot.CaseManagementOpenSlots = 5;

        //    // Execute the conversation.
        //    await CreateTestFlow(UpdateOrganizationDialog.Name, expectedOrganization, expectedSnapshot)
        //        .Test("begin", Phrases.Capacity.GetHousingOpen)                
                .Test(expectedSnapshot.OpenBeds.ToString(), Phrases.CaseManagement.GetCaseManagementOpen)
                .Test(expectedSnapshot.CaseManagementOpenSlots.ToString(), StartsWith(Phrases.JobTrainingServices.GetJobTrainingOpenings))
        //        .Test(expectedSnapshot.BedsOpen.ToString(), Phrases.UpdateOrganization.Closing)
        //        .StartTestAsync();
                .Test("0", StartsWith(Phrases.JobTrainingServices.GetJobTrainingWaitlistPositions))
                .Test("2", Phrases.UpdateOrganization.Closing)

        //    // Snapshot should be completed.
        //    expectedSnapshot.IsComplete = true;

        //    // Validate the results.
        //    await ValidateProfile(expectedOrganization, expectedSnapshot);
        //}

        [Fact]
        public async Task NothingToUpdate()
        {
            var expectedOrganization = CreateDefaultTestOrganization();

            // Execute the conversation.
            await CreateTestFlow(UpdateOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.UpdateOrganization.NothingToUpdate)
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task UpdateOnlyCaseManagement()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.CaseManagementTotal = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.CaseManagementOpenSlots = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateOrganizationDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.CaseManagement.GetCaseManagementOpen)
                .Test(expectedSnapshot.CaseManagementOpenSlots.ToString(), Phrases.UpdateOrganization.Closing)
                .StartTestAsync();

            // Snapshot should be completed.
            expectedSnapshot.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task ClearIncompleteSnapshot()
        {
            var organization = CreateDefaultTestOrganization();
            organization.IsVerified = true;
            organization.HousingEmergencyPrivateTotal = 10;

            var snapshot = new Snapshot(organization.Id);
            snapshot.Date = DateTime.UtcNow.AddDays(-1);

            // Execute the conversation.
            await CreateTestFlow(UpdateOrganizationDialog.Name, organization, snapshot)
                .Send("begin")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Registered)
                .AssertReply(Phrases.Greeting.GetUpdate)
                .StartTestAsync();

            // Validate the results. Snapshot should have been cleared out.
            snapshot = await this.database.GetSnapshot(this.turnContext);
            Assert.Null(snapshot);
        }
    }
}
