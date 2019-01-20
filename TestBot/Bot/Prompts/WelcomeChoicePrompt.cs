using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace TestBot.Bot.Prompts
{
    public class WelcomeChoicePrompt : PromptOptions
    {
        public WelcomeChoicePrompt()
        {
            this.Prompt = Utils.Phrases.Greeting.GetAction;
            this.RetryPrompt = Utils.Phrases.Greeting.GetActionRetry;

            this.Choices = new List<Choice>
            {
                new Choice
                {
                    Value = "New Organization",
                    Synonyms = new List<string> { "new" }
                },
                new Choice
                {
                    Value = "Update Organization",
                    Synonyms = new List<string> { "update" }
                }
            };
        }
    }
}
