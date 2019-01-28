using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderBot.Bot.Dialogs
{
    public abstract class DialogBase
    {
        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly DbInterface database;
        protected readonly IConfiguration configuration;

        public DialogBase(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
        {
            this.state = state;
            this.dialogs = dialogs;
            this.database = database;
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

        private DialogBase CreateFromDialogId(string dialogId)
        {
            // Get the class type. The Id of each dialog is the class name.
            Type type = Type.GetType(dialogId);
            if (type != null && type.IsSubclassOf(typeof(DialogBase)))
            {
                // Create an instance of the dialog and add it to the dialog set.
                return (DialogBase)Activator.CreateInstance(type, this.state, this.dialogs, this.database, this.configuration);
            }

            return null;
        }
    }
}
