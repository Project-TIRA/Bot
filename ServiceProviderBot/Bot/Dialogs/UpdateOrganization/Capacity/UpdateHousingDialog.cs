using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialog : DialogBase
    {
        public static string Name = typeof(UpdateHousingDialog).FullName;

        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
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
                    var organization = await state.Database.GetOrganization(stepContext.Context);              

                    // Validate the numbers.
                    var open = (int)stepContext.Result;
                    if (open > organization.TotalBeds)
                    {
                        // Send error message.
                        var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(organization.TotalBeds));
                        await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the open beds.
                    var snapshot = await state.Database.GetSnapshot(stepContext.Context);
                    snapshot.OpenBeds = (int)stepContext.Result;
                    await state.Database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
