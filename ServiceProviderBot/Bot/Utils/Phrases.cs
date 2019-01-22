using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace ServiceProviderBot.Bot.Utils
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
            public static Activity GetAction = MessageFactory.Text("Welcome! Please choose an option:");
            public static Activity GetActionRetry = MessageFactory.Text("Please choose an option:");
        }

        public static class Location
        {
            public static Activity GetLocation = MessageFactory.Text("In what ZIP code is your oganization? (XXXXX)");
            public static Activity GetLocationError = MessageFactory.Text("Oops, unable to validate location.");
        }

        public static class NewOrganization
        {
            public static Activity GetName = MessageFactory.Text("What is the name of your organization?");
            public static Activity Closing = MessageFactory.Text("Thanks! We will notify you once your organization is verified.");
        }

        public static class UpdateOrganization
        {
            public static Activity NothingToUpdate = MessageFactory.Text("Looks like there isn't anything to update!");
            public static Activity Closing = MessageFactory.Text("Thanks for the update!");
        }
    }
}
