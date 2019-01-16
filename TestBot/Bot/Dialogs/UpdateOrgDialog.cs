using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Models;
using TestBot.Bot.Utils;

namespace TestBot.Bot.Dialogs
{
    public sealed class UpdateOrgDialog : DialogBase
    {
        public static string Name = "UpdateOrgDialog";

        public UpdateOrgDialog(Accessors accessors, DialogSet globalDialogSet) : base(accessors, globalDialogSet)
        {
            // Only set when the bot is initialized.
            if (globalDialogSet != null)
            {
                // The steps this dialog will take.
                WaterfallStep[] waterfallSteps =
                {
                    AgeStepAsync,
                    CleanupAsync
                };

                // Add the dialog to the global dialog set.
                globalDialogSet.Add(new WaterfallDialog(Name, waterfallSteps));
            }
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> AgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(Prompts.IntPrompt, new PromptOptions { Prompt = MessageFactory.Text("Please enter your age.") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> CleanupAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object.
            var profile = await this.accessors.OrganizationProfile.GetAsync(stepContext.Context, () => new OrganizationProfile(), cancellationToken);

            // Update the profile.
            // profile.Age = (int)stepContext.Result;

            // End this dialog to pop it off the stack.
            return await stepContext.EndDialogAsync(cancellationToken);
        }
    }
}
