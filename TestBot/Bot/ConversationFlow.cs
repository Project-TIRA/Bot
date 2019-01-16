using System;
using System.Collections.Generic;
using TestBot.Bot.Dialogs;

namespace TestBot.Bot
{
    public class ConversationFlow
    {
        public int CurrentIndex { get; set; }
        public List<DialogBase> Dialogs { get; set; }

        public ConversationFlow(List<DialogBase> dialogs)
        {
            this.Dialogs = dialogs;
        }
    }
}
