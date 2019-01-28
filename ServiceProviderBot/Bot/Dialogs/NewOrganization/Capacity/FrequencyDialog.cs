using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using ServiceProviderBot.Bot.Prompts;
using ServiceProviderBot.Bot.Utils;
using System;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity
{
    public class FrequencyDialog : DialogBase
    {
        public static string Name = typeof(FrequencyDialog).FullName;

        /// <summary>Creates a dialog for getting housing capacity.</summary>
        /// <param name="state">The state accessors.</param>
        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs, DbInterface database)
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the update frequency.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ChoicePrompt,
                        new UpdateFrequencyPromptOptions(),
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the update frequency.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.UpdateFrequency = Enum.Parse<Frequency>(((FoundChoice)stepContext.Result).Value);
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
