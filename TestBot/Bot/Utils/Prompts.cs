using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Utils
{
    public static class Prompts
    {
        public static string ConfirmPrompt = "ConfirmPrompt";
        public static string IntPrompt = "IntPrompt";
        public static string TextPrompt = "TextPrompt";

        /// <summary>
        /// Adds globally used prompts to the global dialog set
        /// </summary>
        public static void Init(DialogSet globalDialogSet)
        {
            globalDialogSet.Add(new ConfirmPrompt(ConfirmPrompt));
            globalDialogSet.Add(new NumberPrompt<int>(IntPrompt));
            globalDialogSet.Add(new TextPrompt(TextPrompt));
        }
    }
}
