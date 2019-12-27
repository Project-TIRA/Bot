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
        public static int ConvertToTimezoneOffset(string inputString, DateTime offsetTo)
        {
            ParseHourAndMinute(inputString, out DateTime input);

            var timezoneOffset = (input - offsetTo).Hours;

            // Parsing the time can result in different days depending on the locale
            // of the host machine, so if the result is out of the expected range, adjust
            // it to get the correct difference.
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
