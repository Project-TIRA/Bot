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
            public static Activity GetHousingError = MessageFactory.Text("Oops, the total beds must be greater than the open beds.");
            public static Activity GetFrequency = MessageFactory.Text("How often would you like to be contacted to update your capacity?");

            public static string GetHousingErrorFormat(int totalBeds)
            {
                return string.Format("Oops, the total beds ({0}) must be greater than the open beds.", totalBeds);
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
            public static string New = "new";
            public static string Update = "update";
            public static Activity TimeToUpdate = MessageFactory.Text($"It's time to update! Send \"{Update}\" when you are ready to begin.");
            public static Activity Welcome = MessageFactory.Text("Welcome!");
            public static Activity Unregistered = MessageFactory.Text("It looks like you haven't registered your organization yet.");
            public static Activity Registered = MessageFactory.Text("It looks like you have already registered your organization.");
            public static Activity GetNew = MessageFactory.Text($"Send \"{New}\" to register your organization.");
            public static Activity GetUpdate = MessageFactory.Text($"Send \"{Update}\" to update your organization.");
            public static Activity Unverified = MessageFactory.Text("It looks like your organization is still pending verification. You will be notified once your organization is verified.");

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

        public static class CaseManagement
        {
            public static Activity GetHasCaseManagement = MessageFactory.Text("Does your organization offer case management or support services?");
            public static Activity GetHasDemographic = MessageFactory.Text("Do you work with a specific demographic for case management?");
            public static Activity GetHasDemographicMen = MessageFactory.Text("Do you work with men for case management?");
            public static Activity GetHasDemographicWomen = MessageFactory.Text("Do you work with women for case management?");
            public static Activity GetHasDemographicAgeRange = MessageFactory.Text("Does your organization work with an age range for case management?");
            public static Activity GetAgeRangeStart = MessageFactory.Text("What is the youngest age you work with for case management?");
            public static Activity GetAgeRangeEnd = MessageFactory.Text("What is the oldest age you work with for case management?");

            public static Activity GetHasSobriety = MessageFactory.Text("Do you only work with sober individuals for case management?");
            public static Activity GetCaseManagementTotal = MessageFactory.Text("How many TOTAL spaces do you have for case management?");
            public static Activity GetCaseManagementOpen = MessageFactory.Text("How many OPEN spaces do you have for case management?");
            public static Activity GetHasWaitingList = MessageFactory.Text("Do you have a waiting list for case management?");
            public static Activity GetCaseManagementWaitlistLength = MessageFactory.Text("How long is your waiting list?");

            public static string GetCaseManagementSpaceErrorFormat(int totalSpaces)
            {
                return string.Format("Oops, the total slots ({0}) must be greater than the open slots.", totalSpaces);

            }
        }

        public static bool TriggerReset(ITurnContext turnContext)
        {
            // TODO
            return /*!this.configuration.IsProduction() &&*/ string.Equals(turnContext.Activity.Text, "reset", StringComparison.OrdinalIgnoreCase);
        }
    }
}
