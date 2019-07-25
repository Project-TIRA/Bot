using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Dialogs
{
    public abstract class DialogBase
    {
        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly ApiInterface api;
        protected readonly IConfiguration configuration;

        public DialogBase(StateAccessors state, DialogSet dialogs, ApiInterface api, IConfiguration configuration)
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

        protected WaterfallStep[] GenerateUpdateSteps<T>(string serviceName, string totalPropertyName, string openPropertyName,
            string hasWaitlistPropertyName, string waitlistLengthPropertyName, Activity prompt) where T : ModelBase
        {
            return new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest snapshot.
                    var serviceData = await this.api.GetLatestServiceData<T>(Helpers.UserId(stepContext.Context));
                    var totalProperty = (int)typeof(T).GetProperty(totalPropertyName).GetValue(serviceData);

                    // Check if the organization has this service.
                    if (totalProperty > 0)
                    {
                        // Prompt for the open count.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = prompt,
                                RetryPrompt = Phrases.Capacity.RetryInvalidCount(totalProperty, prompt),
                                Validations = totalProperty },
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

                        // Get the latest housing snapshot and update it.
                        var data = await this.api.GetLatestServiceData<T>(Helpers.UserId(stepContext.Context));
                        typeof(T).GetProperty(openPropertyName).SetValue(data, open);
                        await data.Update(this.api);

                        // Check if a waitlist is available.
                        var hasWaitlist = (bool)typeof(T).GetProperty(hasWaitlistPropertyName).GetValue(data);
                        if (hasWaitlist && open == 0)
                        {
                            // Prompt for the waitlist length.
                            return await stepContext.PromptAsync(
                                Utils.Prompts.IntPrompt,
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
                        // Get the latest housing snapshot and update it.
                        var data = await this.api.GetLatestServiceData<T>(Helpers.UserId(stepContext.Context));
                        typeof(T).GetProperty(waitlistLengthPropertyName).SetValue(data, (int)stepContext.Result);
                        await data.Update(this.api);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                }
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
