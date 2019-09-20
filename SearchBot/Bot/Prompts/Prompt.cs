using Microsoft.Bot.Builder.Dialogs;

namespace ServiceProviderBot.Bot.Prompts
{
    public static class Prompt
    {
        public static string TextPrompt = "TextPrompt";

        /// <summary>
        /// Adds each prompt to the master dialog set
        /// </summary>
        public static void Register(DialogSet dialogs)
        {
            dialogs.Add(new TextPrompt(TextPrompt));
        }
    }
}
