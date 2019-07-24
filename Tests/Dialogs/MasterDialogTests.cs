using System;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NewOrganization()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.UpdateFrequency = Frequency.Daily;

            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("new")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(TestOrgPartialAddress, StartsWith(Phrases.Location.GetLocationConfirmation(TestOrgFullAddress)))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", StartsWith(Phrases.MentalHealth.GetHasMentalHealth))
                .Test("no", StartsWith(Phrases.Capacity.GetFrequency))
                .Test(expectedOrganization.UpdateFrequency.ToString(), StartsWith(Phrases.CaseManagement.GetHasCaseManagement))
                .Test("no", StartsWith(Phrases.JobTrainingServices.GetHasJobTraining))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Organization should be completed.
            expectedOrganization.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task UpdateOrganization()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.IsVerified = true;
            expectedOrganization.HousingEmergencyPrivateTotal = 10;
            expectedOrganization.HasJobTrainingServices = true;
            expectedOrganization.TotalJobTrainingPositions = 10;
            expectedOrganization.HasJobTrainingWaitlist = true;
            expectedOrganization.JobTrainingWaitlistPositions = 0;
            expectedOrganization.CaseManagementTotal = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.BedsEmergencyPrivateOpen = 5;
            expectedSnapshot.OpenJobTrainingPositions = 5;
            expectedSnapshot.JobTrainingWaitlistPositions = 2;

            await CreateTestFlow(MasterDialog.Name, expectedOrganization)
                .Send("update")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Capacity.GetHousingEmergencyPrivateOpen)
                .Test("5", Phrases.CaseManagement.GetCaseManagementOpen)
                .Test("5", StartsWith(Phrases.JobTrainingServices.GetJobTrainingOpenings))
                .Test("0", StartsWith(Phrases.JobTrainingServices.GetJobTrainingWaitlistPositions))
                .Test("2", Phrases.UpdateOrganization.Closing)
                .StartTestAsync();

            // Snapshot should be completed.
            expectedSnapshot.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task UpdateOrganizationPendingVerification()
        {
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.IsVerified = false;

            await CreateTestFlow(MasterDialog.Name, initialOrganization)
                .Test("update", Phrases.Greeting.Unverified)
                .StartTestAsync();
        }

        [Fact]
        public async Task AlreadyRegistered()
        {
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.IsVerified = true;

            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name, initialOrganization)
                .Send(Phrases.Greeting.New)
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Registered)
                .StartTestAsync();
        }

        [Fact]
        public async Task Unregistered()
        {
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send(Phrases.Greeting.Update)
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .StartTestAsync();
        }

        [Fact]
        public async Task NonKeywordNewOrganization()
        {
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .AssertReply(Phrases.Greeting.GetNew)
                .Test(Phrases.Greeting.New, Phrases.NewOrganization.GetName)
                .StartTestAsync();
        }

        [Fact]
        public async Task NonKeywordUpdateOrganization()
        {
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.IsVerified = true;

            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name, initialOrganization)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Registered)
                .AssertReply(Phrases.Greeting.GetUpdate)
                .Test(Phrases.Greeting.Update, Phrases.UpdateOrganization.NothingToUpdate)
                .StartTestAsync();
        }

        [Fact]
        public async Task NonKeywordInvalid()
        {
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .AssertReply(Phrases.Greeting.GetNew)
                .Test("hi", Phrases.Greeting.GetNew)
                .StartTestAsync();
        }

        [Fact]
        public async Task Reset()
        {
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .AssertReply(Phrases.Greeting.GetNew)
                .Send("reset")
                .AssertReply(Phrases.Greeting.GetNew)
                .StartTestAsync();
        }
    }
}
