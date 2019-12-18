using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityModel.Helpers
{
    [Flags]
    public enum DayFlags : int
    {
        None = 0,

        Sunday = (1 << 0),
        Monday = (1 << 1),
        Tuesday = (1 << 2),
        Wednesday = (1 << 3),
        Thursday = (1 << 4),
        Friday = (1 << 5),
        Saturday = (1 << 6),

        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        Weekend = Saturday | Sunday,
        Everyday = Weekdays | Weekend
    }

    public static class DayFlagsHelpers
    {
        public static Dictionary<DayFlags, string> StringMappings = new Dictionary<DayFlags, string>()
        {
            { DayFlags.Monday, "M" },
            { DayFlags.Tuesday, "T" },
            { DayFlags.Wednesday, "W" },
            { DayFlags.Thursday, "Th" },
            { DayFlags.Friday, "F" },
            { DayFlags.Saturday, "Sa" },
            { DayFlags.Sunday, "Su" },
        };

        public static DayFlags CurrentDay()
        {
            DateTime today = DateTime.UtcNow;
            switch (today.DayOfWeek)
            {
                case DayOfWeek.Sunday: return DayFlags.Sunday;
                case DayOfWeek.Monday: return DayFlags.Monday;
                case DayOfWeek.Tuesday: return DayFlags.Tuesday;
                case DayOfWeek.Wednesday: return DayFlags.Wednesday;
                case DayOfWeek.Thursday: return DayFlags.Thursday;
                case DayOfWeek.Friday: return DayFlags.Friday;
                case DayOfWeek.Saturday: return DayFlags.Saturday;
                default: return DayFlags.None;
            }
        }

        public static string ToString(DayFlags dayFlags)
        {
            if (dayFlags == DayFlags.Everyday)
            {
                return "everyday";
            }
            else if (dayFlags == DayFlags.Weekdays)
            {
                return "weekdays";
            }
            else if (dayFlags == DayFlags.Weekend)
            {
                return "the weekend";
            }
            else
            {
                return dayFlags.ToString();
            }
        }

        public static bool FromString(string input, string delimiter, out DayFlags dayFlags)
        {
            dayFlags = DayFlags.None;
            var tokens = input.Split(delimiter);

            foreach (var token in tokens)
            {
                var key = StringMappings.FirstOrDefault(m => string.Equals(m.Value, token.Trim(), StringComparison.OrdinalIgnoreCase)).Key;
                if (key == DayFlags.None)
                {
                    dayFlags = DayFlags.None;
                    return false;
                }

                dayFlags |= key;
            }

            return true;
        }
    }
}
