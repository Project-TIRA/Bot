
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
            await CreateTestFlow(MasterDialog.Name, user: null)
                .Test("hi", Phrases.Greeting.NotRegistered)
                .StartTestAsync();
        }

        [Fact]
        public async Task NoOrganization()
        {
            User user = await TestHelpers.CreateUser(this.api, organizationId: string.Empty);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("hi", Phrases.Greeting.NoOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task OrganizationNotVerified()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: false);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Test("hi", Phrases.Greeting.UnverifiedOrganization)
                .StartTestAsync();
        }

        [Fact]
        public async Task Help()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(MasterDialog.Name, user)
                .Send(Phrases.Greeting.HelpKeyword)
                .AssertReply(Phrases.Greeting.Welcome(user))
                .AssertReply(Phrases.Greeting.Help)
                .StartTestAsync();
        }
    }
}
