using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs.UpdateOrganization;
using ServiceProviderBot.Bot.Prompts;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Shared.ApiInterface;
using System;

namespace ServiceProviderBot.Bot.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public MasterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Check if the user is already registered.
                    var user = await api.GetUser(Helpers.GetUserToken(stepContext.Context));
                    if (user == null)
                    {
                        await Messages.SendAsync(Phrases.Greeting.NotRegistered, stepContext.Context, cancellationToken);
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                    // Check if we already have an organization for the user.
                    var organization = await api.GetOrganization(Helpers.GetUserToken(stepContext.Context));
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
                        bool isKeyword =
                            string.Equals(incomingMessage, Phrases.Greeting.EnableKeyword, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(incomingMessage, Phrases.Greeting.DisableKeyword, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(incomingMessage, Phrases.Greeting.UpdateKeyword, StringComparison.OrdinalIgnoreCase);

                        if (isKeyword)
                        {
                            return await stepContext.NextAsync(incomingMessage);
                        }
                    }

                    // Prompt for a keyword.
                    return await stepContext.PromptAsync(
                        Prompt.GreetingTextPrompt,
                        new PromptOptions { Prompt = Phrases.Greeting.Keywords(user.ContactEnabled) },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var result = stepContext.Result as string;

                    if (string.Equals(result, Phrases.Greeting.UpdateKeyword, StringComparison.OrdinalIgnoreCase))
                    {
                        // Push the update organization dialog onto the stack.
                        return await BeginDialogAsync(stepContext, UpdateOrganizationDialog.Name, null, cancellationToken);
                    }
                    else if (string.Equals(result, Phrases.Greeting.EnableKeyword, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(result, Phrases.Greeting.DisableKeyword, StringComparison.OrdinalIgnoreCase))
                    {
                        // Enable/disable contact.
                        var enable = string.Equals(result, Phrases.Greeting.EnableKeyword, StringComparison.OrdinalIgnoreCase);

                        var user = await api.GetUser(Helpers.GetUserToken(stepContext.Context));
                        if (user.ContactEnabled != enable)
                        {
                            user.ContactEnabled = enable;
                            await this.api.Update(user);
                        }

                        await Messages.SendAsync(Phrases.Greeting.ContactEnabledUpdated(user.ContactEnabled), stepContext.Context, cancellationToken);
                        return await stepContext.EndDialogAsync(cancellationToken);
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
