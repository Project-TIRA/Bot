using Luis;
using System.Collections.Generic;

namespace SearchBot.Bot.State
{
    public class ConversationContext
    {
        public Dictionary<string, object> Data { get; set; }

        public ConversationContext()
        {
            this.Data = new Dictionary<string, object>();
        }

        public void AddContext(LuisModel luisModel)
        {

        }
    }
}