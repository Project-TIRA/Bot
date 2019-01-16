using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Dialogs
{
    public sealed class NewOrgDialog : DialogBase
    {
        private const string NamePrompt = "Name";
        private const string NumPrompt = "Num";

        public override string Name { get { return "NewOrg"; } }

        public NewOrgDialog(Accessors accessors, DialogSet globalDialogSet) : base(accessors)
        {
            // Only gets when the bot is initialized.
            if (globalDialogSet != null)
            {
                // The steps this dialog will take.
                WaterfallStep[] waterfallSteps =
                {
                    NameStepAsync,
                    NumStepAsync,
                    CleanupAsync
                };

                // Add each dialog to the global dialog set.
                globalDialogSet.Add(new WaterfallDialog(this.Name, waterfallSteps));
                globalDialogSet.Add(new TextPrompt(NamePrompt));
                globalDialogSet.Add(new NumberPrompt<int>(NumPrompt));
            }
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await stepContext.PromptAsync(NamePrompt, new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> NumStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await this.accessors.UserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            // Update the profile.
            userProfile.Name = (string)stepContext.Result;

            return await stepContext.PromptAsync(NumPrompt, new PromptOptions { Prompt = MessageFactory.Text("Please enter any number.") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> CleanupAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // TODO: Check/save value from previous step
            return await HandleNextStep(stepContext, cancellationToken);
        }
    }
}
