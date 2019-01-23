using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using ServiceProviderBot.Bot.Dialogs.NewOrganization;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using ServiceProviderBot.Bot.Prompts;

namespace ServiceProviderBot.Bot.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public override WaterfallDialog Init(DbModel dbContext, StateAccessors state, DialogSet dialogs)
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the action.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ChoicePrompt,
                        new GreetingPromptOptions(),
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var choice = (FoundChoice)stepContext.Result;

                    // Branch based on the input.
                    switch (choice.Index)
                    {
                        case GreetingPromptOptions.NewOrganizationChoice:
                        {
                            // Push the new organization dialog onto the stack.
                            return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, NewOrganizationDialog.Name, null, cancellationToken);
                        }
                        case GreetingPromptOptions.UpdateOrganizationChoice:
                        {
                            // Push the update organization dialog onto the stack.
                            return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, UpdateOrganizationDialog.Name, null, cancellationToken);
                        }
                        default:
                        {
                            return await stepContext.NextAsync(null, cancellationToken);
                        }
                    }
                },
                async (stepContext, cancellationToken) =>
                {
                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
