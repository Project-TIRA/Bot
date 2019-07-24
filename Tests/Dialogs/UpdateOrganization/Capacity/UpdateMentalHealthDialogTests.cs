using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateMentalHealthDialogTests : DialogTestBase
    {
        [Fact]
        public async Task ValidInPatientTotal()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_InPatientTotal = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Send(expectedSnapshot.MentalHealth_InPatientOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task InvalidOutPatientTotal()
        {
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.MentalHealth_InPatientTotal = 10;
            initialOrganization.MentalHealth_OutPatientTotal = 10;

            var error = string.Format(Phrases.MentalHealth.GetMentalHealthErrorFormat(initialOrganization.MentalHealth_OutPatientTotal));

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, initialOrganization)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test("20", error)
                .StartTestAsync();
        }

        [Fact]
        public async Task ValidOutPatientTotal()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_InPatientTotal = 10;
            expectedOrganization.MentalHealth_OutPatientTotal = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 5;
            expectedSnapshot.MentalHealth_OutPatientOpen = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test(expectedSnapshot.MentalHealth_InPatientOpen.ToString(), Phrases.MentalHealth.GetOutPatientOpen)
                .Send(expectedSnapshot.MentalHealth_OutPatientOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task InvalidInPatientTotal()
        {
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.MentalHealth_InPatientTotal = 10;

            var error = string.Format(Phrases.MentalHealth.GetMentalHealthErrorFormat(initialOrganization.MentalHealth_InPatientTotal));

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, initialOrganization)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test("20", error)
                .StartTestAsync();
        }

        [Fact]
        public async Task ValidInPatientWaitlistRequest()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_HasWaitlist = true;


            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 0;
            expectedSnapshot.MentalHealth_InPatientWaitlistLength = 3;

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test(expectedSnapshot.MentalHealth_InPatientOpen.ToString(), Phrases.MentalHealth.GetInPatientWaitlistLength)
                .Send(expectedSnapshot.MentalHealth_InPatientWaitlistLength.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task InvalidInPatientWaitlistRequest_NoWaitlist()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_HasWaitlist = false;


            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 0;
            expectedSnapshot.MentalHealth_OutPatientOpen = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test(expectedSnapshot.MentalHealth_InPatientOpen.ToString(), Phrases.MentalHealth.GetOutPatientOpen)
                .Send(expectedSnapshot.MentalHealth_OutPatientOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task InvalidInPatientWaitlistRequest_HasOpening()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_InPatientTotal = 10;
            expectedOrganization.MentalHealth_HasWaitlist = true;


            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 1;
            expectedSnapshot.MentalHealth_OutPatientOpen = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test(expectedSnapshot.MentalHealth_InPatientOpen.ToString(), Phrases.MentalHealth.GetOutPatientOpen)
                .Send(expectedSnapshot.MentalHealth_OutPatientOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task InvalidInPatientWaitlistRequest_MoreThanTotal()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_InPatientTotal = 0;
            expectedOrganization.MentalHealth_HasWaitlist = true;


            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 5;

            var error = string.Format(Phrases.MentalHealth.GetMentalHealthErrorFormat(expectedOrganization.MentalHealth_InPatientTotal));

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test(expectedSnapshot.MentalHealth_InPatientOpen.ToString(), error)
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task ValidOutPatientWaitlistRequest()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_InPatientTotal = 10;
            expectedOrganization.MentalHealth_OutPatientTotal = 10;
            expectedOrganization.MentalHealth_HasWaitlist = true;


            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 5;
            expectedSnapshot.MentalHealth_OutPatientOpen = 0;
            expectedSnapshot.MentalHealth_InPatientWaitlistLength = 3;
            expectedSnapshot.MentalHealth_OutPatientWaitlistLength = 3;

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test(expectedSnapshot.MentalHealth_InPatientOpen.ToString(), Phrases.MentalHealth.GetOutPatientOpen)
                .Test(expectedSnapshot.MentalHealth_OutPatientOpen.ToString(), Phrases.MentalHealth.GetOutPatientWaitlistLength)
                .Send(expectedSnapshot.MentalHealth_OutPatientWaitlistLength.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task InvalidOutPatientWaitlistRequest_NoWaitlist()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_InPatientTotal = 10;
            expectedOrganization.MentalHealth_OutPatientTotal = 10;
            expectedOrganization.MentalHealth_HasWaitlist = false;


            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 0;
            expectedSnapshot.MentalHealth_OutPatientOpen = 0;

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test(expectedSnapshot.MentalHealth_InPatientOpen.ToString(), Phrases.MentalHealth.GetOutPatientOpen)
                .Send(expectedSnapshot.MentalHealth_OutPatientOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }

        [Fact]
        public async Task InvalidOutPatientWaitlistRequest_HasOpening()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_InPatientTotal = 10;
            expectedOrganization.MentalHealth_HasWaitlist = true;


            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 1;
            expectedSnapshot.MentalHealth_OutPatientOpen = 5;

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test(expectedSnapshot.MentalHealth_InPatientOpen.ToString(), Phrases.MentalHealth.GetOutPatientOpen)
                .Send(expectedSnapshot.MentalHealth_OutPatientOpen.ToString())
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }
        
        [Fact]
        public async Task InvalidOutPatientWaitlistRequest_MoreThanTotal()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.MentalHealth_InPatientTotal = 10;
            expectedOrganization.MentalHealth_OutPatientTotal = 0;
            expectedOrganization.MentalHealth_HasWaitlist = true;


            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.MentalHealth_InPatientOpen = 5;
            expectedSnapshot.MentalHealth_OutPatientOpen = 5;

            var error = string.Format(Phrases.MentalHealth.GetMentalHealthErrorFormat(expectedOrganization.MentalHealth_OutPatientTotal));

            // Execute the conversation.
            await CreateTestFlow(UpdateMentalHealthDialog.Name, expectedOrganization, expectedSnapshot)
                .Test("begin", Phrases.MentalHealth.GetInPatientOpen)
                .Test(expectedSnapshot.MentalHealth_InPatientOpen.ToString(), Phrases.MentalHealth.GetOutPatientOpen)
                .Test(expectedSnapshot.MentalHealth_OutPatientOpen.ToString(), error)
                .StartTestAsync();

            // Validate the results.
            await ValidateProfileMentalHealth(expectedOrganization, expectedSnapshot);
        }
        
        
    }
}
