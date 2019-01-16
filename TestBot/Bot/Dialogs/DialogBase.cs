using Microsoft.Bot.Builder.Dialogs;

namespace TestBot.Bot.Dialogs
{
    public class DialogBase
    {
        protected Accessors accessors;
        protected DialogSet globalDialogSet;

        public DialogBase(Accessors accessors, DialogSet globalDialogSet)
        {
            this.accessors = accessors;
            this.globalDialogSet = globalDialogSet;
        }
    }
}
