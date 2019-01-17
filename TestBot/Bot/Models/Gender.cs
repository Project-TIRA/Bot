using System;
using System.Collections.Generic;

namespace TestBot.Bot.Models
{
    [Flags]
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
        All = Male | Female
        // Todo: Add more options
    }
}
