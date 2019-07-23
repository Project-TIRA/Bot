using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization.JobTraining
{
    public class JobTrainingDialog : DialogBase
    {
        public static string Name = typeof(JobTrainingDialog).FullName;

        public JobTrainingDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.JobTrainingServices.GetHasJobTraining },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);

                    // Update the profile with whether they have job training services.
                    organization.HasJobTrainingServices = (bool) stepContext.Result;
                    await database.Save();

                    if(organization.HasJobTrainingServices) {
                        // Prompt for how many total job training positions they have.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.JobTrainingServices.GetJobTrainingPositions },
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

                    // Update profile with how many total job training positions they have.
                    organization.TotalJobTrainingPositions = (int) stepContext.Result;
                    await database.Save();

                    // Prompt for how many total job training positions they have.
                    return await stepContext.PromptAsync(
                        Utils.Prompts.ConfirmPrompt,
                        new PromptOptions { Prompt = Phrases.JobTrainingServices.GetHasJobTrainingWaitlist },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);

                    // Update profile with whether they have a job training waitlist.
                    organization.HasJobTrainingWaitlist = (bool) stepContext.Result;
                    await database.Save();

                    // End this dialog to pop it off the stack.
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
