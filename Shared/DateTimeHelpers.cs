using System;
using System.Globalization;

namespace Shared
{
    public static class DateTimeHelpers
    {
        static string[] hourFormats = { "htt", "h tt" };
        static string[] hourMinuteFormats = { "htt", "h tt", "h:mmtt", "h:mm tt" };

        /// <summary>
        /// Parses a datetime string to match only hours.
        /// </summary>
        public static bool ParseHour(string input, out DateTime dateTime)
        {
            return Parse(input, hourFormats, out dateTime);
        }

        /// <summary>
        /// Parses a datetime string to match only hours.
        /// </summary>
        public static bool ParseHourAndMinute(string input, out DateTime dateTime)
        {
            return Parse(input, hourMinuteFormats, out dateTime);
        }

        /// <summary>
        /// Parses a datetime string to match one of the provided formats.
        /// </summary>
        public static bool Parse(string input, string[] formats, out DateTime dateTime)
        {
            return DateTime.TryParseExact(input, formats,  CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        /// <summary>
        /// Parses a datetime string to match one of the provided formats.
        /// </summary>
        public static int ConvertToTimezoneOffset(DateTime input, DateTime offsetTo)
        {
            // Make the dates the same day so that the comparison only considers the times.
            input = new DateTime(offsetTo.Year, offsetTo.Month, offsetTo.Day, input.Hour, input.Minute, input.Second);

            var timezoneOffset = (input - offsetTo).Hours;

            // Keep timezone differences to 12 hours. If it is larger than 12 then it
            // means they are two difference days, but we only care about the offset.
            if (timezoneOffset > 12)
            {
                timezoneOffset -= 24;
            }

            if (timezoneOffset < -12)
            {
                timezoneOffset += 24;
            }

            return timezoneOffset;
        }
    }
}
