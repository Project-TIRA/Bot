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
            var actualTimezoneOffset = DateTimeHelpers.ConvertToTimezoneOffset(localTime.ToShortTimeString(), utcTime);
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
            var hours = new List<string>()
            {
                "12am", "1am", "2am", "3am", "4am", "5am",
                "6am", "7am", "8am", "9am", "10am", "11am",
                "12pm", "1pm", "2pm", "3pm", "4pm", "5pm",
                "6pm", "7pm", "8pm", "9pm", "10pm", "11pm"
            };

            // Go through each hour and offset rotation through these

            var midnight = SetTime(DateTime.Now);

            return new List<object[]>()
            {
                // UtcTime, LocalTime, ExpectedTimezoneOffset
                new object[]{ midnight, "12am", 0 },
                new object[]{ midnight, "1am", 1 },
                new object[]{ midnight, "2am", 2 },
                new object[]{ midnight, "3am", 3 },
                new object[]{ midnight, "4am", 4 },
                new object[]{ midnight, "5am", 5 },
                new object[]{ midnight, "6am", 6 },
                new object[]{ midnight, "7am", 7 },
                new object[]{ midnight, "8am", 8 },
                new object[]{ midnight, "9am", 9 },
                new object[]{ midnight, "10am", 10 },
                new object[]{ midnight, "11am", 11 },
                new object[]{ midnight, "12pm", 12 },
                new object[]{ midnight, "1pm", -11 },
                new object[]{ midnight, "2pm", -10},
                new object[]{ midnight, "3pm", -9 },
                new object[]{ midnight, "4pm", -8 },
                new object[]{ midnight, "5pm", -7 },
                new object[]{ midnight, "6pm", -6 },
                new object[]{ midnight, "7pm", -5 },
                new object[]{ midnight, "8pm", -4 },
                new object[]{ midnight, "9pm", -3 },
                new object[]{ midnight, "10pm", -2 },
                new object[]{ midnight, "11pm", -1 },
            };
        }

        public static DateTime SetTime(DateTime dateTime, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, seconds, milliseconds, dateTime.Kind);
        }
    }
}
