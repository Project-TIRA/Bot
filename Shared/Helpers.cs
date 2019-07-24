
using Microsoft.Bot.Builder;

namespace Shared
{
    public static class Helpers
    {
        public static string UserId(ITurnContext context)
        {
            return "9a0f1731-45ae-e911-a97f-000d3a30da4f";
            //return context.Activity.From.Id;
        }
    }
}
