using ServiceProviderBot.Bot.Dialogs.Preferences;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ServiceProviderBotTests.Dialogs.Preferences
{
    public class TimeDialogTests : DialogTestBase
    {
        [Theory]
        [MemberData(nameof(TestTimes))]
        public async Task Update(string currentTime, bool currentTimeIsValid, string reminderTime, bool reminderTimeIsValid)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            var testFlow = CreateTestFlow(TimeDialog.Name, user)
                .Test(Phrases.Keywords.Time, Phrases.Preferences.GetCurrentTime)
                .Test(currentTime, currentTimeIsValid ? Phrases.Preferences.GetUpdateTime : Phrases.Preferences.GetCurrentTimeRetry);

            if (currentTimeIsValid)
            {
                var parseResult = DateTime.TryParse(reminderTime, out var reminder);

                testFlow = testFlow.Test(reminderTime, reminderTimeIsValid ?
                    Phrases.Preferences.UpdateTimeUpdated(reminder.ToShortTimeString()) :
                    Phrases.Preferences.GetUpdateTimeRetry);
            }

            await testFlow.StartTestAsync();
        }

        [Theory]
        [MemberData(nameof(TestTimezoneOffsets))]
        public void TimezoneOffsets(DateTime utcTime, DateTime localTime, int expectedTimezoneOffset)
        {
            var actualTimezoneOffset = DateTimeHelpers.ConvertToTimezoneOffset(localTime, utcTime);
            Assert.Equal(expectedTimezoneOffset, actualTimezoneOffset);
        }

        public static IEnumerable<object[]> TestTimes()
        {
            return new List<object[]>()
            {
                // CurrentTime, CurrentTimeIsValid, ReminderTime, ReminderTimeIsValid
                new object[] { "test", false, "", false },
                new object[] { "12", false, "", false },
                new object[] { "12:15", false, "", false },
                new object[] { "13 pm", false, "", false },
                new object[] { "12:590am", false, "", false },

                new object[] { "12pm", true, "test", false },
                new object[] { "12 pm", true, "12", false },
                new object[] { "12:00pm", true, "12:15", false },
                new object[] { "3:30am", true, "13 pm", false },
                new object[] { "3:30 am", true, "12:590am", false },
                new object[] { "9:15am", true, "8:30am", false },
                new object[] { "9:15 am", true, "8:30 am", false },

                new object[] { "10 am", true, "8am", true },
                new object[] { "10am", true, "8 am", true },
            };
        }

        public static IEnumerable<object[]> TestTimezoneOffsets()
        {
            // UtcTime, LocalTime, ExpectedTimezoneOffset
            var result = new List<object[]>();

            // Go through each hour of the day.
            for (int utcHour = 0; utcHour < 24; ++utcHour)
            {
                // Use hours and half hours.
                var utcHourDt = SetTime(DateTime.UtcNow, utcHour);
                var utcHalfHourDt = SetTime(DateTime.UtcNow, utcHour, 30);

                // The 11 hours before are -X offset and the 11 hours after are +X offset.
                for (int localHour = -11; localHour <= 11; ++localHour)
                {
                    result.Add(new object[] { utcHourDt, utcHourDt.AddHours(localHour).ToShortTimeString(), localHour });
                    result.Add(new object[] { utcHalfHourDt, utcHalfHourDt.AddHours(localHour).ToShortTimeString(), localHour });
                }

                // Special case: 12 hours can be either -12 or +12.
                // Adding an hour can spill into the next day, so adjust for that case.
                var expectedOffset = utcHour >= 12 ? -12 : 12;
                result.Add(new object[] { utcHourDt, utcHourDt.AddHours(12).ToShortTimeString(), expectedOffset });
                result.Add(new object[] { utcHalfHourDt, utcHalfHourDt.AddHours(12).ToShortTimeString(), expectedOffset });
            }

            return result;
        }

        public static DateTime SetTime(DateTime dateTime, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, seconds, milliseconds, dateTime.Kind);
        }
    }
}
