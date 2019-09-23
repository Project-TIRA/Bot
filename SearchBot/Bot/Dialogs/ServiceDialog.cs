using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using Shared.ApiInterface;

namespace SearchBot.Bot.Dialogs
{
    public class ServiceDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public ServiceDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (dialogContext, cancellationToken) =>
                {
                    // Check if the location is known.

                    // Skip this step.
                    return await dialogContext.NextAsync();
                },
                async (dialogContext, cancellationToken) =>
                {
                    // Check if longterm or emergency is known.

                    // Skip this step.
                    return await dialogContext.NextAsync();
                },
                async (dialogContext, cancellationToken) =>
                {
                    // Check if private or shared-space is known.

                    // End this dialog to pop it off the stack.
                    return await dialogContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
