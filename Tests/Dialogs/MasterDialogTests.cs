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
            /*
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.UpdateFrequency = Frequency.Daily;

            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("new")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.NewOrganization.GetName)
                .Test(expectedOrganization.Name, Phrases.Location.GetLocation)
                .Test(expectedOrganization.Zip, StartsWith(Phrases.Demographic.GetHasDemographic))
                .Test("no", StartsWith(Phrases.Capacity.GetHasHousing))
                .Test("no", StartsWith(Phrases.Capacity.GetFrequency))
                .Test(expectedOrganization.UpdateFrequency.ToString(), Phrases.NewOrganization.Closing)
                .StartTestAsync();

            // Organization should be completed.
            expectedOrganization.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization);
            */
        }

        [Fact]
        public async Task UpdateOrganization()
        {
            /*
            var expectedOrganization = CreateDefaultTestOrganization();
            expectedOrganization.IsVerified = true;
            expectedOrganization.TotalBeds = 10;

            var expectedSnapshot = new Snapshot(expectedOrganization.Id);
            expectedSnapshot.OpenBeds = 5;

            await CreateTestFlow(MasterDialog.Name, expectedOrganization)
                .Send("update")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Capacity.GetHousingOpen)
                .Test("5", Phrases.UpdateOrganization.Closing)
                .StartTestAsync();

            // Snapshot should be completed.
            expectedSnapshot.IsComplete = true;

            // Validate the results.
            await ValidateProfile(expectedOrganization, expectedSnapshot);
            */
        }

        [Fact]
        public async Task UpdateOrganizationPendingVerification()
        {
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.IsVerified = false;

            await CreateTestFlow(MasterDialog.Name, initialOrganization)
                .Test("update", Phrases.Greeting.UnverifiedOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task AlreadyRegistered()
        {
            /*
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.IsVerified = true;

            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name, initialOrganization)
                .Send(Phrases.Greeting.Help)
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Registered)
                .StartTestAsync();
            */
        }

        [Fact]
        public async Task Unregistered()
        {
            /*
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send(Phrases.Greeting.Update)
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .StartTestAsync();
            */
        }

        [Fact]
        public async Task NonKeywordNewOrganization()
        {
            /*
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .AssertReply(Phrases.Greeting.GetHelp)
                .Test(Phrases.Greeting.Help, Phrases.NewOrganization.GetName)
                .StartTestAsync();
            */
        }

        [Fact]
        public async Task NonKeywordUpdateOrganization()
        {
            /*
            var initialOrganization = CreateDefaultTestOrganization();
            initialOrganization.IsVerified = true;

            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name, initialOrganization)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Registered)
                .AssertReply(Phrases.Greeting.Keywords)
                .Test(Phrases.Greeting.Update, Phrases.UpdateOrganization.NothingToUpdate)
                .StartTestAsync();
            */
        }

        [Fact]
        public async Task NonKeywordInvalid()
        {
            /*
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .AssertReply(Phrases.Greeting.GetHelp)
                .Test("hi", Phrases.Greeting.GetHelp)
                .StartTestAsync();
            */
        }

        [Fact]
        public async Task Reset()
        {
            /*
            // Execute the conversation.
            await CreateTestFlow(MasterDialog.Name)
                .Send("hi")
                .AssertReply(Phrases.Greeting.Welcome)
                .AssertReply(Phrases.Greeting.Unregistered)
                .AssertReply(Phrases.Greeting.GetHelp)
                .Send("reset")
                .AssertReply(Phrases.Greeting.GetHelp)
                .StartTestAsync();
            */
        }
    }
}
