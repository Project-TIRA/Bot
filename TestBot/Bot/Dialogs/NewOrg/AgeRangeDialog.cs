using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Utils;

namespace TestBot.Bot.Dialogs.NewOrg
{
    public sealed class AgeRangeDialog : DialogBase
    {
        private const string AgeRangeStartPrompt = "AgeRangeStartPrompt";
        private const string AgeRangeEndPrompt = "AgeRangeEndPrompt";

        public override string Name { get { return "AgeRange"; } }

        public AgeRangeDialog(Accessors accessors, DialogSet globalDialogSet) : base(accessors, globalDialogSet)
        {
            // Only set when the bot is initialized.
            if (globalDialogSet != null)
            {
                // The steps this dialog will take.
                WaterfallStep[] waterfallSteps =
                {
                    AgeRangeStartStepAsync,
                    AgeRangeEndStepAsync,
                    CleanupAsync
                };

                // Add each dialog to the global dialog set.
                globalDialogSet.Add(new WaterfallDialog(this.Name, waterfallSteps));
                globalDialogSet.Add(new NumberPrompt<int>(AgeRangeStartPrompt));
                globalDialogSet.Add(new NumberPrompt<int>(AgeRangeEndPrompt));
            }
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> AgeRangeStartStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                Prompts.IntPrompt,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What is the youngest age your organization works with?")
                },
                cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> AgeRangeEndStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object.
            var profile = await this.accessors.OrganizationProfile.GetAsync(stepContext.Context, () => new OrganizationProfile(), cancellationToken);

            // Update the profile with the result of the previous step.
            profile.AgeRangeStart = (int)stepContext.Result;

            // Prompt for the next step.
            return await stepContext.PromptAsync(
                Prompts.IntPrompt,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What is the oldest age your organization works with?")
                },
                cancellationToken);
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

            // Update the profile with the result of the previous step.
            profile.AgeRangeEnd = (int)stepContext.Result;

            // Go to the next dialog in the conversation.
            return await HandleNextConversationStep(stepContext, cancellationToken);
        }
    }
}
