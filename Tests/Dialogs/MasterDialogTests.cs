using System.Collections.Generic;
using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs;
using Shared;
using Xunit;

namespace Tests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NotRegistered()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("hi", Phrases.Greeting.NotRegistered)
                .StartTestAsync();
        }

        [Fact]
        public async Task NoOrganization()
        {
            var user = CreateTestUser(string.Empty);
            var initialModels = new List<ModelBase>() { user };

            await CreateTestFlow(MasterDialog.Name, initialModels)
                .Test("hi", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task OrganizationNotVerified()
        {
            var organization = CreateTestOrganization(false);
            var user = CreateTestUser(organization.Id);
            var initialModels = new List<ModelBase>() { organization, user };

            await CreateTestFlow(MasterDialog.Name, initialModels)
                .Test("hi", Phrases.Greeting.UnverifiedOrganization)
                .StartTestAsync();
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
