using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using ServiceProviderBot.Bot.Utils;
using Shared;

namespace ServiceProviderBot.Bot.Dialogs.UpdateOrganization.Capacity
{
    public class UpdateMentalHealthDialog : DialogBase
    {
        public static string Name = typeof(UpdateMentalHealthDialog).FullName;

        public UpdateMentalHealthDialog(StateAccessors state, DialogSet dialogs, DbInterface database, IConfiguration configuration)
            : base(state, dialogs, database, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            // Define the dialog and add it to the set
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the open in patient spots
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.MentalHealth.GetInPatientOpen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);              

                    // Validate the numbers
                    var open = (int)stepContext.Result;
                    if (open > organization.MentalHealth_InPatientTotal)
                    {
                        // Send error message
                        var error = string.Format(Phrases.MentalHealth.GetMentalHealthErrorFormat(organization.MentalHealth_InPatientTotal));
                        await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                        // Repeat the dialog
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the open in patient spots 
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    snapshot.MentalHealth_InPatientOpen = (int)stepContext.Result;
                    await database.Save();

                    // Skip this step
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    bool ShouldAskInPatient = snapshot.MentalHealth_InPatientOpen == 0 && organization.MentalHealth_HasWaitlist;
                    if(ShouldAskInPatient){
                        // Prompt for the open in patient waitlist spots
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.MentalHealth.GetInPatientWaitlistLength },
                            cancellationToken);
                    }

                    // Skip this step
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    bool ShouldAskInPatient = snapshot.MentalHealth_InPatientOpen == 0 && organization.MentalHealth_HasWaitlist;
                    if(ShouldAskInPatient){
                        // Validate the numbers
                        var open = (int)stepContext.Result;
                        if (open > organization.MentalHealth_InPatientTotal)
                        {
                            // Send error message
                            var error = string.Format(Phrases.MentalHealth.GetMentalHealthErrorFormat(organization.MentalHealth_InPatientTotal));
                            await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                            // Repeat the dialog
                            return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                        }

                        // Update the profile with the open out patient spots
                        snapshot.MentalHealth_InPatientWaitlistLength = (int)stepContext.Result;
                        await database.Save();
                    }

                    // Skip this step
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // Prompt for the open out patient spots
                    return await stepContext.PromptAsync(
                        Utils.Prompts.IntPrompt,
                        new PromptOptions { Prompt = Phrases.MentalHealth.GetOutPatientOpen },
                        cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);              

                    // Validate the numbers
                    var open = (int)stepContext.Result;
                    if (open > organization.MentalHealth_OutPatientTotal)
                    {
                        // Send error message
                        var error = string.Format(Phrases.MentalHealth.GetMentalHealthErrorFormat(organization.MentalHealth_OutPatientTotal));
                        await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                        // Repeat the dialog
                        return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                    }

                    // Update the profile with the open beds
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    snapshot.MentalHealth_OutPatientOpen = (int)stepContext.Result;
                    await database.Save();

                    // Skip this step
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    bool ShouldAskOutPatient = snapshot.MentalHealth_OutPatientOpen == 0 && organization.MentalHealth_HasWaitlist;
                    if(ShouldAskOutPatient){
                        // Prompt for the open out patient waitlist spots
                        return await stepContext.PromptAsync(
                            Utils.Prompts.IntPrompt,
                            new PromptOptions { Prompt = Phrases.MentalHealth.GetOutPatientWaitlistLength },
                            cancellationToken);
                    }
                    // Skip this step
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    var organization = await database.GetOrganization(stepContext.Context);
                    var snapshot = await database.GetSnapshot(stepContext.Context);
                    bool ShouldAskOutPatient = snapshot.MentalHealth_OutPatientOpen == 0 && organization.MentalHealth_HasWaitlist;
                    if (ShouldAskOutPatient)
                    {
                        // Validate the numbers
                        var open = (int)stepContext.Result;
                        if (open > organization.MentalHealth_OutPatientTotal)
                        {
                            // Send error message
                            var error = string.Format(Phrases.MentalHealth.GetMentalHealthErrorFormat(organization.MentalHealth_OutPatientTotal));
                            await Messages.SendAsync(error, stepContext.Context, cancellationToken);

                            // Repeat the dialog
                            return await stepContext.ReplaceDialogAsync(Name, null, cancellationToken);
                        }

                        // Update the profile with the open out patient waitlist spots
                        snapshot.MentalHealth_OutPatientWaitlistLength = (int)stepContext.Result;
                        await database.Save();
                    }
                    // Skip this step
                    return await stepContext.NextAsync(null, cancellationToken);
                },
                async (stepContext, cancellationToken) =>
                {
                    // End this dialog to pop it off the stack
                    return await stepContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
