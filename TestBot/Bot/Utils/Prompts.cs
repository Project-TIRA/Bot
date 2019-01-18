using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Utils
{
    public static class Prompts
    {
        public static string ConfirmPrompt = "ConfirmPrompt";
        public static string IntPrompt = "IntPrompt";
        public static string TextPrompt = "TextPrompt";

        /// <summary>
        /// Adds each prompt to the master dialog set
        /// </summary>
        public static void Register(DialogSet dialogs)
        {
            dialogs.Add(new ConfirmPrompt(ConfirmPrompt));
            dialogs.Add(new NumberPrompt<int>(IntPrompt));
            dialogs.Add(new TextPrompt(TextPrompt));
        }
    }
}
