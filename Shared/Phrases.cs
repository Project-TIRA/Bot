﻿using EntityModel;
using EntityModel.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared
{
    public static class Phrases
    {
        public const string ProjectName = "Project TIRA";
        public const string WebsiteUrl = "tira.powerappsportals.com";
        public static List<string> ValidChannels = new List<string>() { Channels.Emulator, Channels.Sms };

        public static class Exceptions
        {
            public static string ServiceProvider = $"Sorry, it looks like something went wrong. If this continues to happen, try sending \"{Keywords.Update}\" to start a new update";
            public static string Search = $"Sorry, it looks like something went wrong";
        }

        public static class Keywords
        {
            public const string Update = "update";
            public const string Options = "options";
            public const string Same = "same";
            public const string Latest = "latest";
            public const string Feedback = "feedback";
            public const string Days = "days";
            public const string Time = "time";
            public const string Enable = "enable";
            public const string Disable = "disable";

            public static List<string> List = new List<string>() { Update, Options, Same, Latest, Feedback, Days, Time, Enable, Disable };

            public static string HowToUpdate = $"\"{Update}\" to update your availability";
            public static string HowToOptions = $"\"{Options}\" for more options";
            public static string HowToDuplicateSame = $"\"{Same}\" to confirm your latest availability is still up-to-date";
            public static string HowToCheckLatest = $"\"{Latest}\" to check your latest availability";
            public static string HowToFeedback = $"\"{Feedback}\" to provide feedback";
            public static string HowToChangeDays = $"\"{Days}\" to change the days that the {ProjectName} bot will contact you";
            public static string HowToChangeTime = $"\"{Time}\" to change the time that the {ProjectName} bot will contact you";
            public static string HowToEnable = $"\"{Enable}\" to allow the {ProjectName} bot to contact you";
            public static string HowToDisable = $"\"{Disable}\" to stop the {ProjectName} bot from contacting you";

            public static string HowToUpdateOrOptions = $"Send {HowToUpdate} or {HowToOptions}";
            public static string HowToUpdateDuplicateOrOptions = $"Send {HowToUpdate}, {HowToDuplicateSame}, or {HowToOptions}";
        }

        public static class Greeting
        {
            public static Activity NoOrganization = MessageFactory.Text($"It looks like you aren't connected with an organization. Visit {WebsiteUrl} to register your organization");
            public static Activity UnverifiedOrganization = MessageFactory.Text("It looks like your organization is still pending verification. You will be notified once your organization is verified");

            private static string Welcome(User user)
            {
                return !string.IsNullOrEmpty(user.Name) ? $"Welcome {user.Name}!" : "Welcome!";
            }

            public static Activity InvalidChannel(ITurnContext turnContext)
            {
                return MessageFactory.Text($"Channel \"{turnContext.Activity.ChannelId}\" is not yet supported");
            }

            public static Activity NotRegistered(ITurnContext turnContext)
            {
                return MessageFactory.Text($"It looks like you aren't registered for channel \"{turnContext.Activity.ChannelId}\". Visit {WebsiteUrl} for more information");
            }

            public static Activity GetKeywordsShort(User user, bool welcomeUser = false)
            {
                string greeting = welcomeUser ? (Welcome(user) + Helpers.NewLine) : string.Empty;
                greeting += Keywords.HowToUpdateOrOptions;
                return MessageFactory.Text(greeting);
            }

            public static Activity GetKeywordOptions(User user)
            {
                var greeting =
                    "- Send " + Keywords.HowToDuplicateSame + Helpers.NewLine +
                    "- Send " + Keywords.HowToCheckLatest + Helpers.NewLine +
                    "- Send " + Keywords.HowToChangeDays + Helpers.NewLine +
                    "- Send " + Keywords.HowToChangeTime + Helpers.NewLine +
                    "- Send " + (user.ContactEnabled ? Keywords.HowToDisable : Keywords.HowToEnable) + Helpers.NewLine +
                    "- Send " + Keywords.HowToFeedback;

                return MessageFactory.Text(greeting);
            }

            public static Activity RemindToUpdate(User user, DayFlags day, string latestUpdateString)
            {
                var name = !string.IsNullOrEmpty(user.Name) ? $", {user.Name}" : string.Empty;
                var greeting = string.Empty;

                switch (day)
                {
                    case DayFlags.Monday: greeting = $"Hope you had a great weekend{name}!"; break;
                    default: greeting = $"Happy {day.ToString()}{name}!"; break;
                }

                if (string.IsNullOrEmpty(latestUpdateString))
                {
                    greeting += " " + Keywords.HowToUpdateDuplicateOrOptions;
                }
                else
                {
                    greeting += " Here is the most recent availability for your organization" +
                       Helpers.NewLine +
                       "- - - - -" +
                       Helpers.NewLine +
                       latestUpdateString +
                       Helpers.NewLine +
                       "- - - - -" +
                       Helpers.NewLine +
                       Keywords.HowToUpdateDuplicateOrOptions;
                }

                return MessageFactory.Text(greeting);
            }
        }

        public static class Capacity
        {
            public static Activity GetOpenings(string serviceName)
            {
                return MessageFactory.Text($"How many openings do you have for {serviceName}?");
            }

            public static Activity GetOpeningsRetry(int total, Activity retryPrompt)
            {
                return MessageFactory.Text($"Oops, the openings cannot be more than the total available ({total}). {retryPrompt.Text}");
            }

            public static Activity GetWaitlistIsOpen(string serviceName)
            {
                return MessageFactory.Text($"Is your waitlist open for {serviceName}?");
            }
        }

        public static class Feedback
        {
            public static Activity GetFeedback = MessageFactory.Text($"What would you like to let the {ProjectName} team know?");
            public static Activity Thanks = MessageFactory.Text("Thanks for the feedback!");
        }

        public static class Preferences
        {
            private const string GetCurrentTimeFormat = "\"h:mm am/pm\"";
            private const string GetUpdateTimeFormat = "\"h am/pm\"";
            private const string GetUpdateDaysFormat = "\"M,T,W,Th,F,Sa,Su\"";

            private const string GetCurrentTimeExample = "You can say things like \"8:30 am\" or \"12:15 pm\"";
            private const string GetUpdateTimeExample = "You can say things like \"8 am\" or \"12 pm\"";
            private const string GetUpdateDaysExample = "You can say things like \"M,W,F\", \"weekdays\", \"weekends\", or \"everyday\"";

            private const string Updated = "Your contact preference has been updated";

            public static Activity GetCurrentTime = MessageFactory.Text($"What time is it for you currently? This is to determine your timezone. {GetCurrentTimeExample}");


            public static Activity InitialUpdate = MessageFactory.Text($"Let's set some contact preferences first!");
            public static Activity GetCurrentTimeRetry = MessageFactory.Text($"Oops, the format is {GetCurrentTimeFormat}. {GetCurrentTimeExample}");

            public static Activity GetUpdateTime = MessageFactory.Text($"Which hour of the day would you like to be contacted? {GetUpdateTimeExample}");
            public static Activity GetUpdateTimeRetry = MessageFactory.Text($"Oops, the format is {GetUpdateTimeFormat}. {GetUpdateTimeExample}");

            public static Activity GetUpdateDays = MessageFactory.Text($"Which days of the week would you like to be contacted? {GetUpdateDaysExample}");
            public static Activity GetUpdateDaysRetry = MessageFactory.Text($"Oops, the format is {GetUpdateDaysFormat}. {GetUpdateDaysExample}");

            public static Activity UpdateTimeUpdated(string time)
            {
                return MessageFactory.Text($"{Updated} to {time}!");
            }

            public static Activity UpdateDaysUpdated(DayFlags days)
            {
                return MessageFactory.Text($"{Updated} to {days.ToString()}!");
            }

            public static Activity ContactEnabledUpdated(bool contactEnabled)
            {
                return MessageFactory.Text($"{Updated}! " + (contactEnabled ? Keywords.HowToDisable : Keywords.HowToEnable));
            }
        }

        public static class Update
        {
            public const string All = "All";
            public static Activity Options = MessageFactory.Text("Which service(s) would you like to update?");
            public static Activity NothingToUpdate = MessageFactory.Text("It looks like there isn't anything to update!");
            public static Activity Closing = MessageFactory.Text("Thanks for the update!");
        }
    }
}
