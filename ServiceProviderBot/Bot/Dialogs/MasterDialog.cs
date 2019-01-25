using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Dialogs.NewOrganization;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using ServiceProviderBot.Bot.Utils;
using System;

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
                    // Check if we already have an organization for this user.
                    var organization = await state.Database.GetOrganization(stepContext.Context);
                    bool isExistingOrganization = organization != null;

                    // Check if the organization is verified.
                    if (isExistingOrganization && !organization.IsVerified)
                    {
                        // Not verified.
                        await Messages.SendAsync(Phrases.Greeting.Unverified, stepContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Send the welcome message.
                    await Messages.SendAsync(Phrases.Greeting.Welcome, stepContext.Context, cancellationToken);

                    // Check if the initial message is one of the keywords.
                    var incomingMessage = stepContext.Context.Activity.Text;
                    if (!string.IsNullOrEmpty(incomingMessage))
                    {
                        if (!isExistingOrganization &&
                            string.Equals(incomingMessage, Phrases.Greeting.New, StringComparison.OrdinalIgnoreCase))
                        {
                            initialTextWasKeyword = true;

                            // Push the new organization dialog onto the stack.
                            return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, NewOrganizationDialog.Name, null, cancellationToken);
                        }
                        else if (isExistingOrganization &&
                            string.Equals(incomingMessage, Phrases.Greeting.Update, StringComparison.OrdinalIgnoreCase))
                        {
                            initialTextWasKeyword = true;

                            // Push the update organization dialog onto the stack.
                            return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, UpdateOrganizationDialog.Name, null, cancellationToken);
                        }
                    }

                    // Send the registered/unregistered message.
                    var greeting = isExistingOrganization ? Phrases.Greeting.Registered : Phrases.Greeting.Unregistered;
                    await Utils.Messages.SendAsync(greeting, stepContext.Context, cancellationToken);

                    // Prompt for the action.
                    var prompt = isExistingOrganization ? Phrases.Greeting.GetUpdate : Phrases.Greeting.GetNew;

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

                    if (choice == Phrases.Greeting.New)
                    {
                        // Push the new organization dialog onto the stack.
                        return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, NewOrganizationDialog.Name, null, cancellationToken);
                    }
                    else if (choice == Phrases.Greeting.Update)
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
