using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;

namespace Shared
{
    public static class Phrases
    {
        public class AgeRange
        {
            public static Activity GetAgeRangeStart = MessageFactory.Text("What is the youngest age your organization works with?");
            public static Activity GetAgeRangeEnd = MessageFactory.Text("What is the oldest age your organization works with?");
            public static Activity GetAgeRangeError = MessageFactory.Text("Oops, the oldest age must be greater than youngest age.");
        }

        public static class Capacity
        {
            public static Activity GetHasHousing = MessageFactory.Text("Does your organization offer housing?");
            public static Activity GetHousingTotal = MessageFactory.Text("How many TOTAL beds does your organization have?");
            public static Activity GetHousingOpen = MessageFactory.Text("How many OPEN beds does your organization have?");
            public static Activity GetFrequency = MessageFactory.Text("How often would you like to be contacted to update your capacity?");

            public static Activity GetHousingError(int totalBeds)
            {
                return MessageFactory.Text($"Oops, the total beds ({totalBeds}) must be greater than the open beds.");
            }
        }

        public static class Demographic
        {
            public static Activity GetHasDemographic = MessageFactory.Text("Does your organization work with a specific demographic?");
            public static Activity GetHasDemographicMen = MessageFactory.Text("Does your organization work with men?");
            public static Activity GetHasDemographicWomen = MessageFactory.Text("Does your organization work with women?");
            public static Activity GetHasDemographicAgeRange = MessageFactory.Text("Does your organization work with an age range?");
        }

        public static class Greeting
        {
            public static string HelpKeyword = "help";
            public static string UpdateKeywork = "update";
            public static Activity Welcome = MessageFactory.Text("Welcome!");
            public static Activity NotRegistered = MessageFactory.Text("It looks like you aren't registered - Visit ... to register.");
            public static Activity NoOrganization = MessageFactory.Text("It looks like you aren't connected with an organization. Visit ... to register your organization.");
            public static Activity UnverifiedOrganization = MessageFactory.Text("It looks like your organization is still pending verification. You will be notified once your organization is verified.");
            public static Activity Keywords = MessageFactory.Text($"Send \"{UpdateKeywork}\" to update your organization's current capacity or \"{HelpKeyword}\" for more information.");
            public static Activity GetHelp = MessageFactory.Text($"TODO: Help dialog");
            public static Activity TimeToUpdate = MessageFactory.Text($"It's time to update! Send \"{UpdateKeywork}\" when you are ready to begin.");
        }

        public static class Location
        {
            public static Activity GetLocation = MessageFactory.Text("In what ZIP code is your oganization? (XXXXX)");
            public static Activity GetLocationError = MessageFactory.Text("Oops, unable to validate location.");
        }

        public static class NewOrganization
        {
            public static Activity GetName = MessageFactory.Text("What is the name of your organization?");
            public static Activity Closing = MessageFactory.Text("Thanks! You will be notified once your organization is verified.");
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
