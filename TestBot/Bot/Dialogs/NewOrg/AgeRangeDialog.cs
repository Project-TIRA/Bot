using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Models;
using TestBot.Bot.Utils;

namespace TestBot.Bot.Dialogs.NewOrg
{
    public static class AgeRangeDialog
    {
        public static string Name = "AgeRangeDialog";

        /// <summary>Creates a dialog for adding a new organization.</summary>
        /// <param name="accessors">The state accessors.</param>
        public static Dialog Create(StateAccessors accessors)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    return await stepContext.PromptAsync(Utils.Prompts.IntPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What is the youngest age your organization works with?")
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the result of the previous step.
                    var profile = await accessors.GetOrganizationProfile(stepContext.Context, cancellationToken);
                    profile.Demographic.AgeRange = new AgeRange
                    {
                        Start = (int)stepContext.Result,
                        End = profile.Demographic.AgeRange.Start
                    };

                    return await stepContext.PromptAsync(Utils.Prompts.IntPrompt, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What is the oldest age your organization works with?")
                    },
                    cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var profile = await accessors.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    // Validate the age.
                    var end = (int)stepContext.Result;
                    if (end < profile.Demographic.AgeRange.Start)
                    {
                        // Repeat the dialog.
                        await Utils.Messages.SendAsync("Oops, the oldest age must be greater than youngest age.", stepContext.Context, cancellationToken);
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);

                    }

                    // Update the profile with the result of the previous step.
                    profile.Demographic.AgeRange = new AgeRange
                    {
                        Start = profile.Demographic.AgeRange.Start,
                        End = (int)stepContext.Result
                    };

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
