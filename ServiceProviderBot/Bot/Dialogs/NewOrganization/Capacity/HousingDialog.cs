using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity
{
    public class HousingDialog : DialogBase
    {
        public static string Name = typeof(HousingDialog).FullName;

        public HousingDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the total beds.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHousingTotal },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with the total beds.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.BedsTotal = (int)stepContext.Result;
                    await database.Save();

                    // Prompt for whether or not they keep a waitlist.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.Capacity.GetHasHousingWaitlist },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Update the profile with whether or not they keep a waitlist.
                    var organization = await database.GetOrganization(stepContext.Context);
                    organization.BedsWaitlist = (bool)stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
