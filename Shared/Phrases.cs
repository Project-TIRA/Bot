using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;

namespace Shared
{
    public static class Phrases
    {
        public static string ProjectName = "Project TIRA";
        public static string WebsiteUrl = "tira.powerappsportals.com";

        public static class Greeting
        {
            public static string UpdateKeyword = "update";
            public static string EnableKeyword = "enable";
            public static string DisableKeyword = "disable";

            private static string Enable = $"Send \"{EnableKeyword}\" to allow the {ProjectName} bot to contact you for your availability";
            private static string Disable = $"Send \"{DisableKeyword}\" to stop the {ProjectName} bot from contacting you for your availability";
            private static string Update = $"Send \"{UpdateKeyword}\" to update your availability";

            public static Activity NotRegistered = MessageFactory.Text($"It looks like you aren't registered - Visit {WebsiteUrl} to register and link your mobile phone number");
            public static Activity NoOrganization = MessageFactory.Text($"It looks like you aren't connected with an organization. Visit {WebsiteUrl} to register your organization");
            public static Activity UnverifiedOrganization = MessageFactory.Text("It looks like your organization is still pending verification. You will be notified once your organization is verified");
            public static Activity TimeToUpdate = MessageFactory.Text($"It's time to update! Send \"{UpdateKeyword}\" when you are ready to begin");

            public static Activity Welcome(User user)
            {
                var name = !string.IsNullOrEmpty(user.Name) ? $" {user.Name}" : string.Empty;
                return MessageFactory.Text($"Welcome{name}!");
            }

            public static Activity Keywords(bool contactEnabled)
            {
                return MessageFactory.Text("Options:" + Environment.NewLine +
                    "- " + Update + Environment.NewLine +
                    "- " + (contactEnabled ? Disable : Enable) + Environment.NewLine);
            }

            public static Activity ContactUpdated(bool contactEnabled)
            {
                return MessageFactory.Text($"Your contact preference has been updated. " + (contactEnabled ? Disable : Enable));
            }
        }

        public static class Capacity
        {
            public static Activity GetOpenings(string serviceName)
            {
                return MessageFactory.Text($"How many openings do you have for {serviceName}?");
            }

            public static Activity GetWaitlistLength(string serviceName)
            {
                return MessageFactory.Text($"How long is your waitlist for {serviceName}?");
            }

            public static Activity RetryInvalidCount(int total, Activity retryPrompt)
            {
                return MessageFactory.Text($"Oops, the openings cannot be more than the total available ({total}). {retryPrompt.Text}");
            }
        }

        public static class Services
        {
            public static class CaseManagement
            {
                public static string Name = "case management";
            }

            public static class Housing
            {
                public static string EmergencySharedBeds = "emergency shared-space beds";
                public static string EmergencyPrivateBeds = "emergency private beds";
                public static string LongTermSharedBeds = "long-term shared-space beds";
                public static string LongTermPrivateBeds = "long-term private beds";
            }

            public static class JobTraining
            {
                public static string Name = "job training services";
            }

            public static class MentalHealth
            {
                public static string InPatient = "mental health in-patient services";
                public static string OutPatient = "mental health out-patient services";
            }

            public static class SubstanceUse
            {
                public static string Detox = "substance use detox services";
                public static string InPatient = "substance use in-patient services";
                public static string OutPatient = "substance use out-patient services";
                public static string Group = "substance use group services";
            }
        }

        public static class Update
        {
            public static Activity NothingToUpdate = MessageFactory.Text("It looks like there isn't anything to update!");
            public static Activity Closing = MessageFactory.Text("Thanks for the update!");
        }
    }
}
