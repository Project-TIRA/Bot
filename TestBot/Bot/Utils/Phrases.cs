using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace TestBot.Bot.Utils
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
            public static Activity GetClosing = MessageFactory.Text("Thanks!");
        }

        public static class NewOrganization
        {
            public static Activity GetName = MessageFactory.Text("What is the name of your organization?");
        }
    }
}
