using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace TestBot.Bot.Prompts
{
    public class GreetingPromptOptions : PromptOptions
    {
        public const int NewOrganizationChoice = 0;
        public const int UpdateOrganizationChoice = 1;

        public GreetingPromptOptions()
        {
            this.Prompt = Utils.Phrases.Greeting.GetAction;
            this.RetryPrompt = Utils.Phrases.Greeting.GetActionRetry;

            // TODO: This gets added again if the prompt result is invalid.
            this.Choices = new List<Choice>
            {
                new Choice
                {
                    Value = "New Organization",
                    Synonyms = new List<string> { "new", "n" }
                },
                new Choice
                {
                    Value = "Update Organization",
                    Synonyms = new List<string> { "update", "u" }
                }
            };
        }
    }
}
