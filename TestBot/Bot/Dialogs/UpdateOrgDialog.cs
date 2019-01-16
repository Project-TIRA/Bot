using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Dialogs
{
    public sealed class UpdateOrgDialog : DialogBase
    {
        private const string AgePrompt = "Age";

        public override string Name { get { return "UpdateOrg"; } }

        public UpdateOrgDialog(Accessors accessors, DialogSet globalDialogSet) : base(accessors)
        {
            // Only gets when the bot is initialized.
            if (globalDialogSet != null)
            {
                // The steps this dialog will take.
                WaterfallStep[] waterfallSteps =
                {
                    AgeStepAsync,
                    CleanupAsync
                };

                // Add each dialog to the global dialog set.
                globalDialogSet.Add(new WaterfallDialog(this.Name, waterfallSteps));
                globalDialogSet.Add(new NumberPrompt<int>(AgePrompt));
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
            return await stepContext.PromptAsync(AgePrompt, new PromptOptions { Prompt = MessageFactory.Text("Please enter your age.") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> CleanupAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await this.accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            // Update the profile.
            userProfile.Age = (int)stepContext.Result;

            return await HandleNextStep(stepContext, cancellationToken);
        }
    }
}
