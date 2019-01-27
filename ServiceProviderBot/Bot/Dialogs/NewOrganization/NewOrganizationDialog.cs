using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Capacity;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Demographic;
using ServiceProviderBot.Bot.Dialogs.NewOrganization.Location;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs.NewOrganization
{
    public class NewOrganizationDialog : DialogBase
    {
        public static string Name = typeof(NewOrganizationDialog).FullName;

        public override WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                    async (stepContext, cancellationToken) =>
                    {
                        // Create a new organization to be filled in by NewOrganization process.
                        await state.Database.CreateOrganization(stepContext.Context);

                        // Prompt for the name.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.TextPrompt,
                            new PromptOptions { Prompt = Phrases.NewOrganization.GetName },
                            cancellationToken);
                    },
                    async (stepContext, cancellationToken) =>
                    {
                        // Update the profile with the name.
                        var organization = await state.Database.GetOrganization(stepContext.Context);
                        organization.Name = (string)stepContext.Result;
                        await state.Database.Save();

                        // Push the location dialog onto the stack.
                        return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, LocationDialog.Name, null, cancellationToken);
                    },
                    async (stepContext, cancellationToken) =>
                    {
                        // Prompt for the demographics.
                        return await stepContext.PromptAsync(
                            Utils.Prompts.ConfirmPrompt,
                            new PromptOptions { Prompt = Phrases.Demographic.GetHasDemographic },
                            cancellationToken);
                    },
                    async (stepContext, cancellationToken) =>
                    {
                        if ((bool)stepContext.Result)
                        {
                            // Push the demographic dialog onto the stack.
                            return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, DemographicDialog.Name, null, cancellationToken);
                        }

                        // Update the profile with the default demographics.
                        var organization = await state.Database.GetOrganization(stepContext.Context);
                        organization.Gender = Gender.All;
                        organization.SetDefaultAgeRange();
                        await state.Database.Save();

                        // Skip this step.
                        return await stepContext.NextAsync(null, cancellationToken);
                    },
                    async (stepContext, cancellationToken) =>
                    {
                        // Push the capacity dialog onto the stack.
                        return await Utils.Dialogs.BeginDialogAsync(state, dialogs, stepContext, CapacityDialog.Name, null, cancellationToken);
                    },
                    async (stepContext, cancellationToken) =>
                    {
                        // Mark the organization as complete.
                        var organization = await state.Database.GetOrganization(stepContext.Context);
                        organization.IsComplete = true;

                        // TODO: Only for testing, remove.
                        organization.IsVerified = true;

                        await state.Database.Save();

                        // Send the closing message.
                        await Messages.SendAsync(Phrases.NewOrganization.Closing, stepContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await stepContext.EndDialogAsync(cancellationToken);
                    }
            });
        }
    }
}
