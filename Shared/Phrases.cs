﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Shared.Models;
using System;

namespace Shared
{
    public static class Phrases
    {
        public static class Capacity
        {
            public static Activity GetWaitlistLength(string service)
            {
                return MessageFactory.Text($"How long is your waitlist for {service}?");
            }

            public static Activity RetryInvalidCount(int total, Activity retryPrompt)
            {
                return MessageFactory.Text($"Oops, the availability cannot be more than the total ({total}). {retryPrompt.Text}");
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

            public static class CaseManagement
            {
                public static string ServiceName = "case management";
                public static Activity GetSpotsOpen = MessageFactory.Text($"How many spots for {ServiceName} do you have open?");
            }

            public static class SubstanceUse
            {
                public static string DetoxService = "detox services";
                public static string InPatientService = "in-patient services";
                public static string OutPatientService = "out-patient services";
                public static string GroupService = "group services";
                public static Activity GetDetoxOpen = MessageFactory.Text($"How many open spaces do you have for your {DetoxService}");
                public static Activity GetInPatientOpen = MessageFactory.Text($"How many open spaces do you have for your {InPatientService}");
                public static Activity GetOutPatientOpen = MessageFactory.Text($"How many open spaces do you have for your {OutPatientService}");
                public static Activity GetGroupOpen = MessageFactory.Text($"How many open spaces do you have for your {GroupService}");
            }
        }

        public static class Greeting
        {
            public static string HelpKeyword = "help";
            public static string UpdateKeywork = "update";
            public static Activity NotRegistered = MessageFactory.Text("It looks like you aren't registered - Visit ... to register.");
            public static Activity NoOrganization = MessageFactory.Text("It looks like you aren't connected with an organization. Visit ... to register your organization.");
            public static Activity UnverifiedOrganization = MessageFactory.Text("It looks like your organization is still pending verification. You will be notified once your organization is verified.");
            public static Activity Keywords = MessageFactory.Text($"Send \"{UpdateKeywork}\" to update your organization's current capacity or \"{HelpKeyword}\" for more information.");
            public static Activity GetHelp = MessageFactory.Text($"TODO: Help dialog");
            public static Activity TimeToUpdate = MessageFactory.Text($"It's time to update! Send \"{UpdateKeywork}\" when you are ready to begin.");

            public static Activity Welcome(User user)
            {
                var name = !string.IsNullOrEmpty(user.Name) ? $" {user.Name}" : string.Empty;
                return MessageFactory.Text($"Welcome{name}!");
            }
        }

        public static class UpdateOrganization
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
