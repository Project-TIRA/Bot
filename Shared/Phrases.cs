using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Shared.Models;
using System;

namespace Shared
{
    public static class Phrases
    {
        public static string WebsiteUrl = "tira.powerappsportals.com";

        public static class Greeting
        {
            public static string HelpKeyword = "help";
            public static string UpdateKeyword = "update";
            public static Activity NotRegistered = MessageFactory.Text($"It looks like you aren't registered - Visit {WebsiteUrl} to register");
            public static Activity NoOrganization = MessageFactory.Text($"It looks like you aren't connected with an organization. Visit {WebsiteUrl} to register your organization");
            public static Activity UnverifiedOrganization = MessageFactory.Text("It looks like your organization is still pending verification. You will be notified once your organization is verified");
            public static Activity Keywords = MessageFactory.Text($"Send \"{UpdateKeyword}\" to update your organization's current capacity or \"{HelpKeyword}\" for more information");
            public static Activity GetHelp = MessageFactory.Text("TODO: Help dialog");
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
                return MessageFactory.Text($"How long is your Waitlist for {service}?");
            }

            public static Activity RetryInvalidCount(int total, Activity retryPrompt)
            {
                return MessageFactory.Text($"Oops, the openings cannot be more than the total availablble ({total}). {retryPrompt.Text}");
            }

            public static class CaseManagement
            {
                public static string Service = "case management";
                public static Activity GetSpotsOpen = MessageFactory.Text($"How many openings do you have for {Service}?");
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
                public static string Service = "job training services";
                public static Activity GetServiceOpen = MessageFactory.Text($"How many openings do you have for {Service}?");
            }

            public static class MentalHealth
            {
                public static string InPatient = "mental health in-patient services";
                public static string OutPatient = "mental health out-patient services";
                public static Activity GetInPatientOpen = MessageFactory.Text($"How many openings do you have for {InPatient}?");
                public static Activity GetOutPatientOpen = MessageFactory.Text($"How many openings do you have for {OutPatient}?");
            }

            public static class SubstanceUse
            {
                public static string DetoxService = "substance use detox services";
                public static string InPatientService = "substance use in-patient services";
                public static string OutPatientService = "substance use out-patient services";
                public static string GroupService = "substance use group services";
                public static Activity GetDetoxOpen = MessageFactory.Text($"How many openings do you have for {DetoxService}");
                public static Activity GetInPatientOpen = MessageFactory.Text($"How many openings do you have for {InPatientService}");
                public static Activity GetOutPatientOpen = MessageFactory.Text($"How many openings do you have for {OutPatientService}");
                public static Activity GetGroupOpen = MessageFactory.Text($"How many openings do you have for {GroupService}");
            }
        }

        public static class Update
        {
            public static Activity NothingToUpdate = MessageFactory.Text("It looks like there isn't anything to update!");
            public static Activity Closing = MessageFactory.Text("Thanks for the update!");
        }

        public static bool TriggerReset(ITurnContext turnContext)
        {
            // TODO
            return /*!this.configuration.IsProduction() &&*/ string.Equals(turnContext.Activity.Text, "reset", StringComparison.OrdinalIgnoreCase);
        }
    }
}
