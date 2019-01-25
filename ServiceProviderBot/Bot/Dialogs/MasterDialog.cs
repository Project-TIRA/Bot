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

        private bool initialTextWasKeyword = false;

        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Send the welcome message.
                    await Utils.Messages.SendAsync(Utils.Phrases.Greeting.Welcome, stepContext.Context, cancellationToken);

                    // Check if we already have an org for this user.
                    var organization = await state.GetOrganization(stepContext.Context);
                    bool isExistingOrganization = organization != null;

                    // Check if the initial message is one of the keywords.
                    var incomingMessage = stepContext.Context.Activity.Text;
                    if (!string.IsNullOrEmpty(incomingMessage))
                    {
                        if (incomingMessage == Utils.Phrases.Greeting.New && !isExistingOrganization)
                        {
                            initialTextWasKeyword = true;

                            // Push the new organization dialog onto the stack.
                            return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, NewOrganizationDialog.Name, null, cancellationToken);
                        }
                        else if (incomingMessage == Utils.Phrases.Greeting.Update && isExistingOrganization)
                        {
                            initialTextWasKeyword = true;

                            // Push the update organization dialog onto the stack.
                            return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, UpdateOrganizationDialog.Name, null, cancellationToken);
                        }
                    }

                    // Send the registered/unregistered message.
                    var greeting = isExistingOrganization ? Utils.Phrases.Greeting.Registered : Utils.Phrases.Greeting.Unregistered;
                    await Utils.Messages.SendAsync(greeting, stepContext.Context, cancellationToken);

                    // Prompt for the action.
                    var prompt = isExistingOrganization ? Utils.Phrases.Greeting.GetUpdate : Utils.Phrases.Greeting.GetNew;

                    return await stepContext.PromptAsync(
                        Utils.Prompts.GreetingTextPrompt,
                        new PromptOptions { Prompt = prompt },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (this.initialTextWasKeyword)
                    {
                        // Already handled the choice.
                        return await stepContext.NextAsync(cancellationToken);
                    }

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
