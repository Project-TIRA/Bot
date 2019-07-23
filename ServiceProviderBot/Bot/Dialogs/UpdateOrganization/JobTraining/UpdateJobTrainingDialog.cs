using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.JobTraining
{
    public class UpdateJobTrainingDialog : DialogBase
    {
        public static string Name = typeof(UpdateJobTrainingDialog).FullName;

        public UpdateJobTrainingDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.JobTrainingServices.GetJobTrainingOpenings },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);

                    // Update the profile with how many openings they have for their job training services.
                    var open = (int) stepContext.Result;
                    if (open > organization.TotalJobTrainingPositions)
                    {
                        // Send error message.
                        var error = string.Format(Phrases.JobTrainingServices.GetJobTrainingOpeningsErrorFormat(organization.TotalJobTrainingPositions));
                        await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                        // Repeat the dialog.
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }
                    organization.OpenJobTrainingPositions = open;
                    await database.Save();

                    if(organization.OpenJobTrainingPositions == 0 && organization.HasJobTrainingWaitlist) {
                        // Prompt for how many people are on their waitlist.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.JobTrainingServices.GetJobTrainingWaitlistPositions },
                            cancellationToken);
                    }
                    else
                    {
                        // End dialog now.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }

                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);

                    // Update profile with how many people are on their waitlist.
                    organization.JobTrainingWaitlistPositions = (int) stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
