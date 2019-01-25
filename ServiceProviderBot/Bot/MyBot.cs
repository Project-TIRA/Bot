using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using ServiceProviderBot.Bot.Dialogs;

namespace ServiceProviderBot.Bot
{
    public class TheBot : IBot
    {
        private readonly StateAccessors state;
        private readonly DialogSet dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="TheBot"/> class.
        /// </summary>
        /// <param name="state">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state</param>
        public TheBot(StateAccessors state)
        {
            this.state = state ?? throw new System.ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            // Register prompts.
            Utils.Prompts.Register(this.dialogs);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Establish context for our dialog from the turn context.
            DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);
            DialogTurnResult results = await Utils.Dialogs.ContinueDialogAsync(this.state, this.dialogs, dialogContext, cancellationToken);

            if (turnContext.Activity.Type == ActivityTypes.Message &&
                results.Status == DialogTurnStatus.Empty)
            {
                await Utils.Dialogs.BeginDialogAsync(this.state, this.dialogs, dialogContext, MasterDialog.Name, null, cancellationToken);
            }
        }
    }
}
