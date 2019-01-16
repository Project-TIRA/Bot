using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Utils;

namespace TestBot.Bot.Dialogs.NewOrg
{
    public sealed class NewOrgDialog : DialogBase
    {
        public override string Name { get { return "NewOrg"; } }

        public NewOrgDialog(Accessors accessors, DialogSet globalDialogSet) : base(accessors, globalDialogSet)
        {
            // Only set when the bot is initialized.
            if (globalDialogSet != null)
            {
                // The steps this dialog will take.
                WaterfallStep[] waterfallSteps =
                {
                    NameStepAsync,
                    SizeStepAsync,
                    AgeRangeStepAsync,
                    AgeRangeConfirmStepAsync,
                    CleanupAsync
                };

                // Add each dialog to the global dialog set.
                globalDialogSet.Add(new WaterfallDialog(this.Name, waterfallSteps));
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
            return await stepContext.PromptAsync(
                Prompts.TextPrompt,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What is the name of your organization?")
                },
                cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> SizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object.
            var profile = await this.accessors.OrganizationProfile.GetAsync(stepContext.Context, () => new OrganizationProfile(), cancellationToken);

            // Update the profile with the result of the previous step.
            profile.Name = (string)stepContext.Result;

            // Prompt for the next step.
            return await stepContext.PromptAsync(
                Prompts.IntPrompt,
                new PromptOptions 
                {
                    Prompt = MessageFactory.Text("What is the size of your organization?"),
                    RetryPrompt = MessageFactory.Text("How many people work for your organization?")
                },
                cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> AgeRangeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object.
            var profile = await this.accessors.OrganizationProfile.GetAsync(stepContext.Context, () => new OrganizationProfile(), cancellationToken);

            // Update the profile with the result of the previous step.
            profile.Size = (int)stepContext.Result;

            // Prompt for the next step.
            return await stepContext.PromptAsync(
                Prompts.ConfirmPrompt,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Does your organization work with an age range?")
                },
                cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> AgeRangeConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                // Push the age range dialog onto the stack.
                var conversationFlow = (ConversationFlow)stepContext.Options;
                //conversationFlow.AddStep(new NewOrgDialog(this.accessors, this.globalDialogSet));

                // TODO: Need to begin this rather than replace
                return await stepContext.NextAsync();
            }
            else
            {
                // Skip this step.
                return await stepContext.NextAsync();
            }
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
            profile.Size = (int)stepContext.Result;

            // Go to the next dialog in the conversation.
            return await HandleNextConversationStep(stepContext, cancellationToken);
        }
    }
}
