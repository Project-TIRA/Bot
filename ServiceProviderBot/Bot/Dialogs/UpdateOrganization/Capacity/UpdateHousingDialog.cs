using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateHousingDialog : DialogBase
    {
        public static string Name = typeof(UpdateHousingDialog).FullName;

        public UpdateHousingDialog(StateAccessors state, DialogSet dialogs, ApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set.
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Get the latest housing snapshot.
                    var housingData = await this.api.GetLatestHousingServiceData(Helpers.UserId(stepContext.Context));

                    // Check longterm private beds.
                    if (housingData.LongTermPrivateBedsTotal > 0)
                    {
                        // Prompt for the open beds.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.LessThanOrEqualPrompt,
                            new PromptOptions { Prompt = Phrases.Capacity.GetHousingOpen,
                                RetryPrompt = Phrases.Capacity.GetHousingError(housingData.LongTermPrivateBedsTotal),
                                Validations = housingData.LongTermPrivateBedsTotal },
                            cancellationToken);
                    }

                    // Skip this step.
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    if (stepContext.Result != null)
                    {
                        // Get the latest housing snapshot and update it.
                        var housingData = await this.api.GetLatestHousingServiceData(Helpers.UserId(stepContext.Context));
                        housingData.LongTermPrivateBedsOpen = int.Parse((string)stepContext.Result);
                        bool success = await housingData.Update(this.api);
                    }

                    //var organization = await api.GetUserOrganization(Helpers.UserId(stepContext.Context));              

                    /*
                    // Validate the numbers.
                    var open = (int)stepContext.Result;
                    if (open > organization.TotalBeds)
                    {
                        // Send error message.
                        var error = string.Format(Phrases.Capacity.GetHousingErrorFormat(organization.TotalBeds));
                        await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the open beds.
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    snapshot.OpenBeds = (int)stepContext.Result;
                    await database.Save();
                    */

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
