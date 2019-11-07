using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;

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
            public const string Feedback = "feedback";
            public const string Enable = "enable";
            public const string Disable = "disable";
            public const string Options = "options";

            public static List<string> List = new List<string>() { Update, Feedback, Enable, Disable };

            public static string HowToUpdate = $"Send \"{Update}\" to update your availability";
            public static string HowToUpdateOrOptions = $"Send \"{Update}\" to update your availability or \"{Options}\" for more options";
            public static string HowToFeedback = $"Send \"{Feedback}\" to provide feedback";
            public static string HowToEnable = $"Send \"{Enable}\" to allow the {ProjectName} bot to contact you for your availability";
            public static string HowToDisable = $"Send \"{Disable}\" to stop the {ProjectName} bot from contacting you for your availability";
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

            public static Activity RemindToUpdate(User user, Day day)
            {
                var name = !string.IsNullOrEmpty(user.Name) ? $", {user.Name}" : string.Empty;
                var greeting = string.Empty;

                switch (day)
                {
                    case Day.Monday: greeting = $"Hope you had a great weekend{name}!"; break;
                    default: greeting = $"Happy {day.ToString()}{name}!"; break;
                }

                return MessageFactory.Text(greeting + " " + Keywords.HowToUpdateOrOptions);
            }

            public static Activity GetKeywords(User user, bool welcomeUser = false)
            {
                string greeting = welcomeUser ? (Welcome(user) + Environment.NewLine) : string.Empty;
                greeting += "- " + Keywords.HowToUpdate + Environment.NewLine +
                            "- " + Keywords.HowToFeedback + Environment.NewLine +
                            "- " + (user.ContactEnabled ? Keywords.HowToDisable : Keywords.HowToEnable);

                return MessageFactory.Text(greeting);
            }

            public static Activity ContactEnabledUpdated(bool contactEnabled)
            {
                return MessageFactory.Text($"Your contact preference has been updated. " + (contactEnabled ? Keywords.HowToDisable : Keywords.HowToEnable));
            }
        }

        public static class Capacity
        {
            public static Activity GetOpenings(string serviceName)
            {
                return MessageFactory.Text($"How many openings do you have for {serviceName}?");
            }

            public static Activity GetWaitlistIsOpen(string serviceName)
            {
                return MessageFactory.Text($"Is your waitlist open for {serviceName}?");
            }

            public static Activity RetryInvalidCount(int total, Activity retryPrompt)
            {
                return MessageFactory.Text($"Oops, the openings cannot be more than the total available ({total}). {retryPrompt.Text}");
            }
        }

        public static class Feedback
        {
            public static Activity GetFeedback = MessageFactory.Text($"What would you like to let the {ProjectName} team know?");
            public static Activity Thanks = MessageFactory.Text("Thanks for the feedback!");
        }

        public static class Search
        {
            public static Activity GetServiceType = MessageFactory.Text("What type of service are you looking for?");
            public static Activity GetHousingType = MessageFactory.Text("What type of housing are you looking for?");

            public static Activity GetLocation(string serviceType)
            {
                return MessageFactory.Text($"In what city are you looking for {(string.IsNullOrEmpty(serviceType) ? "services" : serviceType)}?");
            }
        }

        public static class Services
        {
            public static string All = "All";

            public static class CaseManagement
            {
                public const string ServiceName = "Case Management";
            }

            public static class Housing
            {
                public const string ServiceName = "Housing";
                public const string Emergency = "Emergency";
                public const string LongTerm = "Long-term";
                public const string EmergencySharedBeds = "Emergency Shared-Space Beds";
                public const string EmergencyPrivateBeds = "Emergency Private Beds";
                public const string LongTermSharedBeds = "Long-term shared-space Beds";
                public const string LongTermPrivateBeds = "Long-term Private Beds";
            }

            public static class Employment
            {
                public const string ServiceName = "Employment";
                public const string JobReadinessTraining = "Job Readiness Training";
                public const string PaidInternships = "Paid Internships";
                public const string VocationalTraining = "Vocational Training";
                public const string EmploymentPlacement = "Employment Placement";
            }

            public static class MentalHealth
            {
                public const string ServiceName = "Mental Health";
                public const string InPatient = "Mental Health In-Patient";
                public const string OutPatient = "Mental Health Out-Patient";
            }

            public static class SubstanceUse
            {
                public const string ServiceName = "Substance Use";
                public const string Detox = "Substance Use Detox";
                public const string InPatient = "Substance Use In-Patient";
                public const string OutPatient = "Substance Use Out-Patient";
                public const string Group = "Substance Use Group Services";
            }

            public static Activity Responsestart = MessageFactory.Text("Here's an organization that can help with");
            public static Activity Response(string service, string location, string orgname)
            {
                return MessageFactory.Text($"{Responsestart.Text} {service} in {location} {orgname}");
            }
        }

        public static class Update
        {
            public static Activity Options = MessageFactory.Text("Which service(s) would you like to update?");
            public static Activity NothingToUpdate = MessageFactory.Text("It looks like there isn't anything to update!");
            public static Activity Closing = MessageFactory.Text("Thanks for the update!");
        }

        public static class Intents
        {
            public static Activity Unknown = MessageFactory.Text("Sorry, I'm not able to understand that. What services can I help you find?");
        }
    }
}
