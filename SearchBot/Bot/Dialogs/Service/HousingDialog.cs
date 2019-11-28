using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using SearchBot.Bot.State;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using Shared.Prompts;
using System.Collections.Generic;

namespace SearchBot.Bot.Dialogs.Service
{
    public class HousingDialog : DialogBase
    {
        public static string Name = typeof(HousingDialog).FullName;

        public HousingDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override WaterfallDialog GetWaterfallDialog()
        {
            return new WaterfallDialog(Name, new WaterfallStep[]
            {
                async (dialogContext, cancellationToken) =>
                {
                    // Check if housing was mentioned but the type was not specified.
                    var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);

                    if (conversationContext.IsServiceInvalid(ServiceType.Housing))
                    {
                        // Prompt for the type of housing.
                        var choices = new List<Choice>() { new Choice { Value = Phrases.Services.Housing.Emergency }, new Choice { Value = Phrases.Services.Housing.LongTerm } };

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions {
                                Prompt = Phrases.Search.GetHousingType,
                                Choices = choices
                            },
                            cancellationToken);
                    }

                    // Skip this step.
                    return await dialogContext.NextAsync();
                },
                async (dialogContext, cancellationToken) =>
                {
                    if (dialogContext.Result != null && dialogContext.Result is FoundChoice)
                    {
                        var conversationContext = await this.state.GetConversationContext(dialogContext.Context, cancellationToken);

                        // Update the type of housing.
                        switch (((FoundChoice)dialogContext.Result).Value)
                        {
                            case Phrases.Services.Housing.Emergency: conversationContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.HousingEmergency); break;
                            case Phrases.Services.Housing.LongTerm: conversationContext.CreateOrUpdateServiceContext(ServiceType.Housing, ServiceFlags.HousingLongTerm); break;
                        }
                    }

                    // End this dialog to pop it off the stack.
                    return await dialogContext.EndDialogAsync(cancellationToken);
                }
            });
        }
    }
}
