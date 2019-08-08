using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;

namespace Shared
{
    public static class Phrases
    {
        public static string WebsiteUrl = "tira.powerappsportals.com";

        public static class Greeting
        {
            public static string HelpKeyword = "info";
            public static string UpdateKeyword = "update";
            public static Activity NotRegistered = MessageFactory.Text($"It looks like you aren't registered - Visit {WebsiteUrl} to register and link your mobile phone number");
            public static Activity NoOrganization = MessageFactory.Text($"It looks like you aren't connected with an organization. Visit {WebsiteUrl} to register your organization");
            public static Activity UnverifiedOrganization = MessageFactory.Text("It looks like your organization is still pending verification. You will be notified once your organization is verified");
            public static Activity Keywords = MessageFactory.Text($"Send \"{UpdateKeyword}\" to update your organization's current capacity or \"{HelpKeyword}\" for more information");
            public static Activity Help = MessageFactory.Text($"Project TIRA is a Trafficking Interruption Resource Agent that provides a realtime view of available resource. Visit {WebsiteUrl} for more info");
            public static Activity TimeToUpdate = MessageFactory.Text($"It's time to update! Send \"{UpdateKeyword}\" when you are ready to begin");

            public static Activity Welcome(User user)
            {
                var name = !string.IsNullOrEmpty(user.Name) ? $" {user.Name}" : string.Empty;
                return MessageFactory.Text($"Welcome{name}!");
            }
        }

        public static class Capacity
        {
            public static Activity GetWaitlistLength(string service)
            {
                return MessageFactory.Text($"How long is your waitlist for {service}?");
            }

            public static Activity RetryInvalidCount(int total, Activity retryPrompt)
            {
                return MessageFactory.Text($"Oops, the openings cannot be more than the total available ({total}). {retryPrompt.Text}");
            }

            public static class CaseManagement
            {
                public static string ServiceName = "case management";
                public static Activity GetOpen = MessageFactory.Text($"How many openings do you have for {ServiceName}?");
            }

            public static class Housing
            {
                public static string EmergencySharedBeds = "emergency shared-space beds";
                public static string EmergencyPrivateBeds = "emergency private beds";
                public static string LongTermSharedBeds = "long-term shared-space beds";
                public static string LongTermPrivateBeds = "long-term private beds";
                public static Activity GetEmergencySharedBedsOpen = MessageFactory.Text($"How many {EmergencySharedBeds} do you have open?");
                public static Activity GetEmergencyPrivateBedsOpen = MessageFactory.Text($"How many {EmergencyPrivateBeds} do you have open?");
                public static Activity GetLongTermSharedBedsOpen = MessageFactory.Text($"How many {LongTermSharedBeds} do you have open?");
                public static Activity GetLongTermPrivateBedsOpen = MessageFactory.Text($"How many {LongTermPrivateBeds} do you have open?");
            }

            public static class JobTraining
            {
                public static string ServiceName = "job training services";
                public static Activity GetOpen = MessageFactory.Text($"How many openings do you have for {ServiceName}?");
            }

            public static class MentalHealth
            {
                public static string InPatientName = "mental health in-patient services";
                public static string OutPatientName = "mental health out-patient services";
                public static Activity GetInPatientOpen = MessageFactory.Text($"How many openings do you have for {InPatientName}?");
                public static Activity GetOutPatientOpen = MessageFactory.Text($"How many openings do you have for {OutPatientName}?");
            }

            public static class SubstanceUse
            {
                public static string DetoxServiceName = "substance use detox services";
                public static string InPatientServiceName = "substance use in-patient services";
                public static string OutPatientServiceName = "substance use out-patient services";
                public static string GroupServiceName = "substance use group services";
                public static Activity GetDetoxOpen = MessageFactory.Text($"How many openings do you have for {DetoxServiceName}");
                public static Activity GetInPatientOpen = MessageFactory.Text($"How many openings do you have for {InPatientServiceName}");
                public static Activity GetOutPatientOpen = MessageFactory.Text($"How many openings do you have for {OutPatientServiceName}");
                public static Activity GetGroupOpen = MessageFactory.Text($"How many openings do you have for {GroupServiceName}");
            }
        }

        public static class Update
        {
            public static Activity NothingToUpdate = MessageFactory.Text("It looks like there isn't anything to update!");
            public static Activity Closing = MessageFactory.Text("Thanks for the update!");
        }
    }
}
