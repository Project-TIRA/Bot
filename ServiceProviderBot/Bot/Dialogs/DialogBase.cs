using EntityModel;
using Microsoft.Bot.Builder.Dialogs;

namespace ServiceProviderBot.Bot.Dialogs
{
    public abstract class DialogBase
    {
        public virtual WaterfallDialog Init(StateAccessors state, DialogSet dialogs)
        {
            return null;
        }
    }
}
