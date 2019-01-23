using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity
{
    public class HousingDialog : DialogBase
    {
        public static string Name = typeof(HousingDialog).FullName;

        /// <summary>Creates a dialog for getting housing capacity.</summary>
        /// <param name="state">The state accessors.</param>
        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the total beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHousingTotal },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the total beds.
                    var organization = await state.GetOrganization(stepContext.Context);
                    organization.TotalBeds = (int)stepContext.Result;
                    await state.SaveDbContext();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
