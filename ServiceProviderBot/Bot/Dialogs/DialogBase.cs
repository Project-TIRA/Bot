using EntityModel;
using Microsoft.Bot.Builder.Dialogs;
using ServiceProviderBot.Bot.Utils;

namespace ServiceProviderBot.Bot.Dialogs
{
    public abstract class DialogBase
    {
        public virtual WaterfallDialog Init(StateAccessors state, DialogSet dialogs, DbInterface database)
        {
            return null;
        }
    }
}
