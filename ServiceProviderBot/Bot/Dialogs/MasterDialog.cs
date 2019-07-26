using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using ServiceProviderBot.Bot.Utils;
using Shared;
using System;

namespace ServiceProviderBot.Bot.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public MasterDialog(StateAccessors state, DialogSet dialogs, ApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Check if the user is already registered.
                    var user = await api.GetUser(Helpers.GetPhoneNumber(stepContext.Context));
                    if (user == null)
                    {
                        await Messages.SendAsync(Phrases.Greeting.NotRegistered, stepContext.Context, cancellationToken);
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Check if we already have an organization for this user.
                    var organization = await api.GetOrganization(user.Id);
                    if (organization == null)
                    {
                        await Messages.SendAsync(Phrases.Greeting.NoOrganization, stepContext.Context, cancellationToken);
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Check if the organization is verified.
                    if (!organization.IsVerified)
                    {
                        await Messages.SendAsync(Phrases.Greeting.UnverifiedOrganization, stepContext.Context, cancellationToken);
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Send the welcome message.
                    await Messages.SendAsync(Phrases.Greeting.Welcome(user), stepContext.Context, cancellationToken);

                    // Check if the initial message is one of the keywords.
                    var incomingMessage = stepContext.Context.Activity.Text;
                    if (!string.IsNullOrEmpty(incomingMessage))
                    {
                        if (string.Equals(incomingMessage, Phrases.Greeting.HelpKeyword, StringComparison.OrdinalIgnoreCase))
                        {
                            // Show help dialog.
                            await Messages.SendAsync(Phrases.Greeting.GetHelp, stepContext.Context, cancellationToken);
                            return await stepContext.EndDialogAsync(cancellationToken);
                        }
                        else if (string.Equals(incomingMessage, Phrases.Greeting.UpdateKeyword, StringComparison.OrdinalIgnoreCase))
                        {
                            // Push the update organization dialog onto the stack.
                            return await BeginDialogAsync(stepContext, UpdateOrganizationDialog.Name, null, cancellationToken);
                        }
                    }

                    return await stepContext.PromptAsync(
                        Utils.Prompts.GreetingTextPrompt,
                        new PromptOptions { Prompt = Phrases.Greeting.Keywords },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var result = stepContext.Result as string;
                    if (result == null)
                    {
                        // Initial text was keyword so prompt wasn't needed.
                        return await stepContext.NextAsync(cancellationToken);
                    }

                    if (string.Equals(result, Phrases.Greeting.HelpKeyword, StringComparison.OrdinalIgnoreCase))
                    {
                        // Show help dialog.
                        await Messages.SendAsync(Phrases.Greeting.GetHelp, stepContext.Context, cancellationToken);
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }
                    else if (string.Equals(result, Phrases.Greeting.UpdateKeyword, StringComparison.OrdinalIgnoreCase))
                    {
                        // Push the update organization dialog onto the stack.
                        return await BeginDialogAsync(stepContext, UpdateOrganizationDialog.Name, null, cancellationToken);
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
