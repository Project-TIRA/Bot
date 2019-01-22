using Microsoft.Bot.Builder.Dialogs;
using TestBot.Bot.Prompts;

namespace TestBot.Bot.Utils
{
    public static class Prompts
    {
        public static string ChoicePrompt = "ChoicePrompt";
        public static string ConfirmPrompt = "ConfirmPrompt";
        public static string IntPrompt = "IntPrompt";
        public static string TextPrompt = "TextPrompt";
        public static string LocationTextPrompt = "LocationTextPrompt";

        /// <summary>
        /// Adds each prompt to the master dialog set
        /// </summary>
        public static void Register(DialogSet dialogs)
        {
            dialogs.Add(new ChoicePrompt(ChoicePrompt));
            dialogs.Add(new ConfirmPrompt(ConfirmPrompt));
            dialogs.Add(new NumberPrompt<int>(IntPrompt));
            dialogs.Add(new TextPrompt(TextPrompt));
            dialogs.Add(new TextPrompt(LocationTextPrompt, LocationPromptValidator.Create()));
        }
    }
}
