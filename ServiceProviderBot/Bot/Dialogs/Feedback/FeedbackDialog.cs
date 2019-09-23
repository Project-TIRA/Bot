using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity;
using ServiceProviderBot.Bot.Prompts;
using Shared;
using Shared.ApiInterface;
using System.Collections.Generic;

namespace ServiceProviderBot.Bot.Dialogs.Feedback
{
    public class FeedbackDialog : DialogBase
    {
        public static string Name = typeof(FeedbackDialog).FullName;

        public FeedbackDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (dialogContext, cancellationToken) =>
                {
                    // Prompt for feedback.
                    return await dialogContext.PromptAsync(
                        Prompt.TextPrompt,
                        new PromptOptions {
                            Prompt = Phrases.Feedback.GetFeedback
                        },
                        cancellationToken);
                },
                async (dialogContext, cancellationToken) =>
                {
                    var user = await api.GetUser(dialogContext.Context);

                    // Save the feedback.
                    var feedback = new EntityModel.Feedback();
                    feedback.SenderId = user.Id;
                    feedback.Text = (string)dialogContext.Result;
                    await this.api.Create(feedback);

                    // Send thanks.
                    await Messages.SendAsync(Phrases.Feedback.Thanks, dialogContext.Context, cancellationToken);

                    // End this dialog to pop it off the stack.
                    return await dialogContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
