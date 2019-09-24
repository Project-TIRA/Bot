using Luis;
using System.Collections.Generic;

namespace SearchBot.Bot.State
{
    public class ConversationContext
    {
        public string Location { get; set; }

        public bool EmergencyHousing { get; set; }
        public bool LongtermHousing { get; set; }
        public bool Housing { get; set; }

        public void AddContext(LuisModel luisModel)
        {
            if (luisModel.Entities.Location.Length > 0)
            {
                this.Location = luisModel.Entities.Location[0];
            }

            this.Housing = luisModel.Entities.Housing.Length > 0;
            this.EmergencyHousing = luisModel.Entities.Emergency.Length > 0;
            this.LongtermHousing = luisModel.Entities.Longterm.Length > 0;
        }
    }
}