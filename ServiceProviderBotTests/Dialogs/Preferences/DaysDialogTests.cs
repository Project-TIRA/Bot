using EntityModel.Helpers;
using ServiceProviderBot.Bot.Dialogs.Preferences;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ServiceProviderBotTests.Dialogs.Preferences
{
    public class DaysDialogTests : DialogTestBase
    {
        [Theory]
        [MemberData(nameof(TestDays))]
        public async Task Update(string reminderDays, bool reminderDaysIsValid)
        {
            var organization = await TestHelpers.CreateOrganization(this.api, isVerified: true);
            var user = await TestHelpers.CreateUser(this.api, organization.Id);

            DayFlagsHelpers.FromString(reminderDays, ",", out var dayFlags);

            var testFlow = CreateTestFlow(DaysDialog.Name, user)
                .Test(Phrases.Keywords.Days, Phrases.Preferences.GetUpdateDays)
                .Test(reminderDays, reminderDaysIsValid ? Phrases.Preferences.UpdateDaysUpdated(dayFlags) : Phrases.Preferences.GetUpdateDaysRetry);

            await testFlow.StartTestAsync();

            user = await this.api.GetUser(this.turnContext);
            Assert.Equal(user.ReminderFrequency, dayFlags);
        }

        public static IEnumerable<object[]> TestDays()
        {
            return new List<object[]>()
            {
                // ReminderDays, ReminderDaysIsValid
                new object[] { "test", false },
                new object[] { "mon,tues", false },

                new object[] { "Sa,Su", true },
                new object[] { "M,W,F", true },
                new object[] { "m,t,w,th,f", true },
                new object[] { "M,T,W,Th,F,Sa,Su", true },
                new object[] { "Saturday", true },
                new object[] { "Monday,Wednesday", true },
                new object[] { "Weekdays", true },
                new object[] { "weekends", true },
                new object[] { "Everyday", true },
            };
        }
    }
}
