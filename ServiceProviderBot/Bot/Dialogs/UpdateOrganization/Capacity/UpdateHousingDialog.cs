using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialog : DialogBase
    {
        public static string Name = typeof(UpdateHousingDialog).FullName;

        public override WaterfallDialog Init(DbModel dbContext, StateAccessors state, DialogSet dialogs)
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the open beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHousingOpen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var profile = await state.GetOrganizationProfile(stepContext.Context, cancellationToken);

                    // Validate the numbers.
                    var open = (int)stepContext.Result;
                    if (open > profile.Capacity.Beds.Total)
                    {
                        // Send error message.
                        var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(profile.Capacity.Beds.Total));
                        await Utils.Messages.SendAsync(error, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the open beds.
                    profile.Capacity.Beds.Open = (int)stepContext.Result;

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
