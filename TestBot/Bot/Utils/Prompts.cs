using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Utils
{
    public static class Prompts
    {
        public static string ConfirmPrompt = "ConfirmPrompt";
        public static string IntPrompt = "IntPrompt";
        public static string TextPrompt = "TextPrompt";

        public static void AddGlobalPrompts(DialogSet globalDialogSet)
        {
            globalDialogSet.Add(new NumberPrompt<int>(IntPrompt));
        }
    }
}
