using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Prompts;

namespace ServiceProviderBot.Bot.Utils
{
    public static class Prompts
    {
        public static string ChoicePrompt = "ChoicePrompt";
        public static string ConfirmPrompt = "ConfirmPrompt";
        public static string IntPrompt = "IntPrompt";
        public static string TextPrompt = "TextPrompt";
        public static string GreetingTextPrompt = "GreetingTextPrompt";
        public static string LocationTextPrompt = "LocationTextPrompt";

        /// <summary>
        /// Adds each prompt to the master dialog set
        /// </summary>
        public static void Register(StateAccessors state, DialogSet dialogs, DbInterface database)
        {
            dialogs.Add(new ChoicePrompt(ChoicePrompt));
            dialogs.Add(new ConfirmPrompt(ConfirmPrompt));
            dialogs.Add(new NumberPrompt<int>(IntPrompt));
            dialogs.Add(new TextPrompt(TextPrompt));
            dialogs.Add(new TextPrompt(GreetingTextPrompt, GreetingPromptValidator.Create(state, database)));
            dialogs.Add(new TextPrompt(LocationTextPrompt, LocationPromptValidator.Create()));
        }
    }
}
