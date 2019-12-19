using ServiceProviderBot.Bot.Dialogs.Feedback;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace ServiceProviderBotTests.Dialogs.Feedback
{
    public class FeedbackDialogTests : DialogTestBase
    {
        [Fact]
        public async Task Feedback()
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            await CreateTestFlow(FeedbackDialog.Name, user)
                .Test(Phrases.Keywords.Feedback, Phrases.Feedback.GetFeedback)
                .Test(Phrases.Keywords.Feedback, Phrases.Feedback.Thanks)
                .StartTestAsync();
        }
    }
}
