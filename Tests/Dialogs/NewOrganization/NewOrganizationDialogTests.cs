using System;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.NewOrganization
{
    public class NewOrganizationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task YesToAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.AgeRangeStart = 14;
            expectedOrganization.AgeRangeEnd = 24;
            expectedOrganization.TotalBeds = 10;
            expectedOrganization.UpdateFrequency = Frequency.Daily;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("yes", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expectedOrganization.AgeRangeStart.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Test(expectedOrganization.AgeRangeEnd.ToString(), StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expectedOrganization.TotalBeds.ToString(), StartsWith(Phrases.Capacity.GetFrequency))
                .Test(expectedOrganization.UpdateFrequency.ToString(), StartsWith(Phrases.JobTrainingServices.GetHasJobTraining))
                .Test("yes", StartsWith(Phrases.JobTrainingServices.GetJobTrainingPositions))
                .Test("10", StartsWith(Phrases.JobTrainingServices.GetHasJobTrainingWaitlist))
                .Test("yes", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Organization profile should be completed.
            expectedOrganization.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoToAll()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.UpdateFrequency = Frequency.Daily;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", StartsWith(Phrases.Capacity.GetFrequency))
                .Test(expectedOrganization.UpdateFrequency.ToString(), StartsWith(Phrases.JobTrainingServices.GetHasJobTraining))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Organization profile should be completed.
            expectedOrganization.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoDemographic()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.UpdateFrequency = Frequency.Daily;
            expectedOrganization.TotalBeds = 10;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expectedOrganization.TotalBeds.ToString(), StartsWith(Phrases.Capacity.GetFrequency))
                .Test(expectedOrganization.UpdateFrequency.ToString(), StartsWith(Phrases.JobTrainingServices.GetHasJobTraining))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Organization profile should be completed.
            expectedOrganization.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoAgeRange()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.TotalBeds = 10;
            expectedOrganization.UpdateFrequency = Frequency.Daily;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("yes", Phrases.Capacity.GetHousingTotal)
                .Test(expectedOrganization.TotalBeds.ToString(), StartsWith(Phrases.Capacity.GetFrequency))
                .Test(expectedOrganization.UpdateFrequency.ToString(), StartsWith(Phrases.JobTrainingServices.GetHasJobTraining))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Organization profile should be completed.
            expectedOrganization.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task NoHousing()
        {
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.AgeRangeStart = 14;
            expectedOrganization.AgeRangeEnd = 24;
            expectedOrganization.UpdateFrequency = Frequency.Daily;

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicMen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicWomen))
                .Test("yes", StartsWith(Phrases.Demographic.GetHasDemographicAgeRange))
                .Test("yes", Phrases.AgeRange.GetAgeRangeStart)
                .Test(expectedOrganization.AgeRangeStart.ToString(), Phrases.AgeRange.GetAgeRangeEnd)
                .Test(expectedOrganization.AgeRangeEnd.ToString(), StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", StartsWith(Phrases.Capacity.GetFrequency))
                .Test(expectedOrganization.UpdateFrequency.ToString(), StartsWith(Phrases.JobTrainingServices.GetHasJobTraining))
                .Test("no", Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Organization profile should be completed.
            expectedOrganization.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task ClearIncompleteOrganization()
        {
            var organization = CreateDefaultTestOrganization();
            organization.DateCreated = DateTime.UtcNow.AddDays(-1);

            // Execute the conversation.
            await CreateTestFlow(NewOrganizationDialog.Name, organization)
                .Send("begin")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .AssertReply(Phrases.Greeting.GetNew)
                .StartTestAsync();

            // Validate the results. Organization should have been cleared out.
            organization = await this.database.GetOrganization(this.turnContext);
            Assert.Null(organization);
        }
    }
}
