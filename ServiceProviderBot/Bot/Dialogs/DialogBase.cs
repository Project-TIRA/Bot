using EntityModel;
using Microsoft.Bot.Builder.Dialogs;

namespace ServiceProviderBot.Bot.Dialogs
{
    public abstract class DialogBase
    {
        public virtual WaterfallDialog Init(DbModel dbContext, StateAccessors state, DialogSet dialogs)
        {
            return null;
        }
    }
}
