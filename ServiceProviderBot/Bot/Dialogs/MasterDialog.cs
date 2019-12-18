using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Dialogs.Capacity;
using ServiceProviderBot.Bot.Dialogs.Feedback;
using ServiceProviderBot.Bot.Dialogs.Preferences;
using ServiceProviderBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public MasterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Check if the user is already registered.
                        var user = await api.GetUser(dialogContext.Context);
                        if (user == null)
                        {
                            await Messages.SendAsync(Phrases.Greeting.NotRegistered(dialogContext.Context), dialogContext.Context, cancellationToken);
                            return await dialogContext.EndDialogAsync(cancellationToken);
                        }

                        // Check if we already have an organization for the user.
                        var organization = await api.GetOrganization(user.OrganizationId);
                        if (organization == null)
                        {
                            await Messages.SendAsync(Phrases.Greeting.NoOrganization, dialogContext.Context, cancellationToken);
                            return await dialogContext.EndDialogAsync(cancellationToken);
                        }

                        // Check if the organization is verified.
                        if (!organization.IsVerified)
                        {
                            await Messages.SendAsync(Phrases.Greeting.UnverifiedOrganization, dialogContext.Context, cancellationToken);
                            return await dialogContext.EndDialogAsync(cancellationToken);
                        }

                        // Save the user ID and organization ID to the user context so that
                        // they can be accessed by other dialogs without API lookups.
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.UserId = user.Id;
                        userContext.OrganizationId = user.OrganizationId;

                        // Check if the initial message is one of the keywords.
                        var incomingMessage = dialogContext.Context.Activity.Text;
                        if (!string.IsNullOrEmpty(incomingMessage))
                        {
                            bool isKeyword = Phrases.Keywords.List.Any(k => string.Equals(incomingMessage, k, StringComparison.OrdinalIgnoreCase));
                            if (isKeyword)
                            {
                                return await dialogContext.NextAsync(incomingMessage);
                            }
                        }

                        // Prompt for a keyword.
                        return await dialogContext.PromptAsync(
                            Prompt.GreetingTextPrompt,
                            new PromptOptions {
                                Prompt = Phrases.Greeting.GetKeywordsShort(user, welcomeUser: true),
                                RetryPrompt = Phrases.Greeting.GetKeywordsShort(user)
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = dialogContext.Result as string;

                        if (string.Equals(result, Phrases.Keywords.Update, StringComparison.OrdinalIgnoreCase))
                        {
                            // Push the update dialog onto the stack.
                            return await BeginDialogAsync(dialogContext, UpdateCapacityDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Keywords.Options, StringComparison.OrdinalIgnoreCase))
                        {
                            // Send the options message.
                            var user = await api.GetUser(dialogContext.Context);
                            await Messages.SendAsync(Phrases.Greeting.GetKeywordsWithOptions(user), dialogContext.Context, cancellationToken);
                            return await dialogContext.EndDialogAsync(cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Keywords.Latest, StringComparison.OrdinalIgnoreCase))
                        {
                            // Send the latest availability message.
                            var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                            var latestUpdateString = await Helpers.GetLatestUpdateString(this.api, userContext.OrganizationId);

                            await Messages.SendAsync(latestUpdateString, dialogContext.Context, cancellationToken);
                            return await dialogContext.EndDialogAsync(cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Keywords.Enable, StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(result, Phrases.Keywords.Disable, StringComparison.OrdinalIgnoreCase))
                        {
                            // Enable/disable contact.
                            var enable = string.Equals(result, Phrases.Keywords.Enable, StringComparison.OrdinalIgnoreCase);

                            var user = await api.GetUser(dialogContext.Context);
                            if (user.ContactEnabled != enable)
                            {
                                user.ContactEnabled = enable;
                                await this.api.Update(user);
                            }

                            await Messages.SendAsync(Phrases.Preferences.ContactEnabledUpdated(user.ContactEnabled), dialogContext.Context, cancellationToken);
                            return await dialogContext.EndDialogAsync(cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Keywords.Days, StringComparison.OrdinalIgnoreCase))
                        {
                            // Push the update days dialog onto the stack.
                            return await BeginDialogAsync(dialogContext, DaysDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Keywords.Time, StringComparison.OrdinalIgnoreCase))
                        {
                            // Push the update time dialog onto the stack.
                            return await BeginDialogAsync(dialogContext, TimeDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Keywords.Feedback, StringComparison.OrdinalIgnoreCase))
                        {
                            // Push the feedback dialog onto the stack.
                            return await BeginDialogAsync(dialogContext, FeedbackDialog.Name, null, cancellationToken);
                        }

                        return await dialogContext.NextAsync(cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(cancellationToken);
                    }
                });
            });
        }
    }
}
