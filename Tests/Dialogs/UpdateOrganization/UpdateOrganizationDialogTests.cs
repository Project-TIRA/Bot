using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Dialogs.UpdateOrganization
{
    public class UpdateOrganizationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NothingToUpdate()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(UpdateOrganizationDialog.Name, user)
                .Test("test", Phrases.Update.NothingToUpdate)
                .StartTestAsync();
        }
    }
}
