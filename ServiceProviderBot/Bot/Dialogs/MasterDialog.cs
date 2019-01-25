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

        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Check if we already have an org for this user.
                    var organization = await state.GetOrganization(stepContext.Context);
                    
                    // Check if the initial message is one of the keywords.
                    var incomingMessage = (string)stepContext.Result;
                    if (!string.IsNullOrEmpty(incomingMessage))
                    {
                        if (incomingMessage == Utils.Phrases.Greeting.New && organization == null)
                        {
                            // Push the new organization dialog onto the stack.
                            return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, NewOrganizationDialog.Name, null, cancellationToken);
                        }
                        else if (incomingMessage == Utils.Phrases.Greeting.Update && organization != null)
                        {
                            // Push the update organization dialog onto the stack.
                            return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, UpdateOrganizationDialog.Name, null, cancellationToken);
                        }
                    }

                    // Send the greeting.
                    var greeting = organization == null ? Utils.Phrases.Greeting.Unregistered : Utils.Phrases.Greeting.Registered;
                    await Utils.Messages.SendAsync(greeting, stepContext.Context, cancellationToken);

                    // Prompt for the action.
                    var prompt = organization == null ? Utils.Phrases.Greeting.GetNew : Utils.Phrases.Greeting.GetUpdate;

                    return await stepContext.PromptAsync(
                        Utils.Prompts.TextPrompt,
                        new PromptOptions { Prompt = prompt },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var choice = (string)stepContext.Result;

                    if (choice == Utils.Phrases.Greeting.New)
                    {
                        // Push the new organization dialog onto the stack.
                        return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, NewOrganizationDialog.Name, null, cancellationToken);
                    }
                    else if (choice == Utils.Phrases.Greeting.Update)
                    {
                        // Push the update organization dialog onto the stack.
                        return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, UpdateOrganizationDialog.Name, null, cancellationToken);
                    }

                    return await stepContext.NextAsync(cancellationToken);
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
