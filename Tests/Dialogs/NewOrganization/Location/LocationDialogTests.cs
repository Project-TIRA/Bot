using System.Threading.Tasks;
using EntityModel;
using Microsoft.Bot.Schema;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Location;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Xunit;

namespace Tests.Dialogs.NewOrganization.Location
{
    public class LocationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task CorrectAddress()
        {
            var expectedOrganization = CreateDefaultTestOrganization();

            // Execute the conversation.
            await CreateTestFlow(LocationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.Location.GetLocation)
                .Test(TestOrgPartialAddress, StartsWith(Phrases.Location.GetLocationConfirmation(TestOrgFullAddress)))
                .Send("yes")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task WrongAddress()
        {
            var expectedOrganization = CreateDefaultTestOrganization();

            // Execute the conversation.
            await CreateTestFlow(LocationDialog.Name, expectedOrganization)
                .Test("begin", Phrases.Location.GetLocation)
                .Test(TestOrgPartialAddress, StartsWith(Phrases.Location.GetLocationConfirmation(TestOrgFullAddress)))
                .Test("no", Phrases.Location.GetLocation)
                .Test(TestOrgPartialAddress, StartsWith(Phrases.Location.GetLocationConfirmation(TestOrgFullAddress)))
                .Send("yes")
                .StartTestAsync();

            // Validate the results.
            await ValidateProfile(expectedOrganization);
        }

        [Fact]
        public async Task Invalid()
        {
            // Execute the conversation.
            await CreateTestFlow(LocationDialog.Name)
                .Test("begin", Phrases.Location.GetLocation)
                .Test("0000000000", Phrases.Location.GetLocationError)
                .StartTestAsync();
        }

        [Fact]
        public async Task NotFound()
        {
            // Execute the conversation.
            await CreateTestFlow(LocationDialog.Name)
                .Test("begin", Phrases.Location.GetLocation)
                .Test("12345", Phrases.Location.GetLocationError)
                .StartTestAsync();
        }
    }
}
