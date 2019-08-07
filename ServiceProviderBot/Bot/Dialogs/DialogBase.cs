using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Prompts;
using Shared;
using Shared.ApiInterface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Dialogs
{
    public abstract class DialogBase
    {
        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly IApiInterface api;
        protected readonly IConfiguration configuration;

        public DialogBase(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
        {
            this.state = state;
            this.dialogs = dialogs;
            this.api = api;
            this.configuration = configuration;
        }

        public virtual WaterfallDialog GetWaterfallDialog()
        {
            return null;
        }

        /// <summary>
        /// JIT creates the dialog if necessary and begins the dialog.
        /// Type <typeparamref name="T"/> is the type of dialog, deriving from <see cref="DialogBase"/>
        /// </summary>
        public async Task<DialogTurnResult> BeginDialogAsync(DialogContext context, string dialogId, object options, CancellationToken cancellationToken)
        {
            // Only create the dialog if it doesn't exist.
            if (dialogs.Find(dialogId) == null)
            {
                var dialog = CreateFromDialogId(dialogId);
                if (dialog != null)
                {
                    dialogs.Add(dialog.GetWaterfallDialog());
                }
            }

            return await context.BeginDialogAsync(dialogId, options, cancellationToken);
        }

        /// <summary>
        /// JIT creates the dialog stack if necessary and continues the dialog.
        /// Type <typeparamref name="T"/> is the type of dialog, deriving from <see cref="DialogBase"/>
        /// </summary>
        public async Task<DialogTurnResult> ContinueDialogAsync(DialogContext context, CancellationToken cancellationToken)
        {
            // Go through each entry in the context stack.
            foreach (var entry in context.Stack)
            {
                // Only create the dialog if it doesn't exist.
                if (dialogs.Find(entry.Id) == null)
                {
                    var dialog = CreateFromDialogId(entry.Id);
                    if (dialog != null)
                    {
                        dialogs.Add(dialog.GetWaterfallDialog());
                    }
                }
            }

            return await context.ContinueDialogAsync(cancellationToken);
        }

        protected WaterfallStep GenerateCreateDataStep<T>() where T : ServiceModelBase, new()
        {
            return async (stepContext, cancellationToken) =>
            {
                var userToken = Helpers.GetUserToken(stepContext.Context);
                var user = await this.api.GetUser(userToken);

                // Get the latest snapshot.
                var previousData = await this.api.GetLatestServiceData<T>(userToken);

                // Create a new snapshot and copy the static values from the previous one.
                var data = new T();
                data.CopyStaticValues(previousData);
                data.CreatedById = user.Id;
                await this.api.Create(data);

                // Continue to the next step.
                return await stepContext.NextAsync(cancellationToken);
            };
        }

        protected WaterfallStep[] GenerateUpdateSteps<T>(string serviceName, string totalPropertyName, string openPropertyName,
            string hasWaitlistPropertyName, string waitlistLengthPropertyName, Activity prompt) where T : ServiceModelBase, new()
        {
            return new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Get the service.
                    var service = await this.api.GetService<T>(Helpers.GetUserToken(stepContext.Context));

                    // Get the latest snapshot created by the user.
                    var data = await this.api.GetLatestServiceData<T>(Helpers.GetUserToken(stepContext.Context), createdByUser: true);
                    var totalPropertyValue = (int)typeof(T).GetProperty(totalPropertyName).GetValue(data);

                    // Check if the organization has this service.
                    if (totalPropertyValue > 0)
                    {
                        var validations = new LessThanOrEqualPromptValidations()
                        {
                            Max = totalPropertyValue
                        };

                        // Prompt for the open count.
                        return await stepContext.PromptAsync(
                            Prompt.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = prompt,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(totalPropertyValue, prompt),
                                Validations = validations },
                            cancellationToken);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Check if the previous step had a result.
                    if (stepContext.Result != null)
                    {
                        var open = int.Parse((string)stepContext.Result);

                        // Get the latest snapshot created by the user and update it.
                        var data = await this.api.GetLatestServiceData<T>(Helpers.GetUserToken(stepContext.Context), createdByUser: true);
                        typeof(T).GetProperty(openPropertyName).SetValue(data, open);
                        await this.api.Update(data);

                        // Check if a waitlist is available.
                        var hasWaitlist = (bool)typeof(T).GetProperty(hasWaitlistPropertyName).GetValue(data);
                        if (hasWaitlist && open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Prompt.IntPrompt,
                                new PromptOptions { Prompt = Phrases.Capacity.GetWaitlistLength(serviceName) },
                                cancellationToken);
                        }
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Check if the previous step had a result.
                    if (stepContext.Result != null)
                    {
                        // Get the latest snapshot created by the user and update it.
                        var data = await this.api.GetLatestServiceData<T>(Helpers.GetUserToken(stepContext.Context), createdByUser: true);
                        typeof(T).GetProperty(waitlistLengthPropertyName).SetValue(data, (int)stepContext.Result);
                        await this.api.Update(data);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                }
            };
        }

        protected WaterfallStep GenerateCompleteDataStep<T>() where T : ServiceModelBase, new()
        {
            return async (stepContext, cancellationToken) =>
            {
                // Mark the snapshot created by the user as complete.
                var data = await this.api.GetLatestServiceData<T>(Helpers.GetUserToken(stepContext.Context), createdByUser: true);
                data.IsComplete = true;
                await this.api.Update(data);

                // Continue to the next step.
                return await stepContext.NextAsync(cancellationToken);
            };
        }

        private DialogBase CreateFromDialogId(string dialogId)
        {
            // Get the class type. The Id of each dialog is the class name.
            Type type = Type.GetType(dialogId);
            if (type != null && type.IsSubclassOf(typeof(DialogBase)))
            {
                // Create an instance of the dialog and add it to the dialog set.
                return (DialogBase)Activator.CreateInstance(type, this.state, this.dialogs, this.api, this.configuration);
            }

            return null;
        }
    }
}
