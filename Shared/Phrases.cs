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
            // onboarding
            public static Activity GetHasHousing = MessageFactory.Text("Does your organization offer housing?");
            public static Activity GetHasHousingEmergency = MessageFactory.Text("Does your organization have beds for emergency housing?");
            public static Activity GetHasHousingLongterm = MessageFactory.Text("Does your organization have beds for longterm housing?");
            public static Activity GetHousingEmergencyPrivateTotal = MessageFactory.Text("How many TOTAL EMERGENCY beds in PRIVATE rooms does your organization have?");
            public static Activity GetHousingEmergencySharedTotal = MessageFactory.Text("How many TOTAL EMERGENCY beds in SHARED rooms does your organization have?");
            public static Activity GetHousingLongtermPrivateTotal = MessageFactory.Text("How many TOTAL LONGTERM beds in PRIVATE rooms does your organization have?");
            public static Activity GetHousingLongtermSharedTotal = MessageFactory.Text("How many TOTAL LONGTERM beds in SHARED rooms does your organization have?");
            public static Activity GetFrequency = MessageFactory.Text("How often would you like to be contacted to update your capacity?");

            // updating
            public static Activity GetHousingEmergencyPrivateOpen = MessageFactory.Text("How many OPEN EMERGENCY beds does your organization have in PRIVATE rooms?");
            public static Activity GetHousingEmergencySharedOpen = MessageFactory.Text("How many OPEN EMERGENCY beds does your organization have in SHARED rooms?");
            public static Activity GetHousingLongtermPrivateOpen = MessageFactory.Text("How many OPEN LONGTERM beds does your organization have in PRIVATE rooms?");
            public static Activity GetHousingLongtermSharedOpen = MessageFactory.Text("How many OPEN LONGTERM beds does your organization have in SHARED rooms?");
            public static Activity GetHousingError = MessageFactory.Text("Oops, the total beds must be greater than the open beds.");
            public static Activity GetHousingEmergencyPrivateWaitlist = MessageFactory.Text("How long is your waitlist for emergency beds in private rooms?");
            public static Activity GetHousingEmergencySharedWaitlist = MessageFactory.Text("How long is your waitlist for emergency beds in shared rooms?");
            public static Activity GetHousingLongtermPrivateWaitlist = MessageFactory.Text("How long is your waitlist for longterm beds in private rooms?");
            public static Activity GetHousingLongtermSharedWaitlist = MessageFactory.Text("How long is your waitlist for longterm beds in shared rooms?");

            public static string GetHousingErrorFormat(int totalBeds)
            {
                return string.Format("Oops, the total beds ({0}) must be greater than the open beds. Please input the number of open beds again.", totalBeds);
            }
        }

        public static class HousingDemographic
        {
            public static Activity GetHasDemographicMen = MessageFactory.Text("Does your organization house men?");
            public static Activity GetHasDemographicWomen = MessageFactory.Text("Does your organization house women?");
            public static Activity GetHasDemographicNonbinary = MessageFactory.Text("Does your organization house non-binary people?");
            public static Activity GetHasDemographicAgeRange = MessageFactory.Text("Is your organization’s housing restricted to a specific age range?");
            public static Activity GetAgeRangeStart = MessageFactory.Text("What is the youngest age your organization will house?");
            public static Activity GetAgeRangeEnd = MessageFactory.Text("What is the oldest age your organization will house?");
            public static Activity GetHasDemographicPregnant = MessageFactory.Text("Does your organization house people who are pregnant?");
            public static Activity GetHasDemographicParenting = MessageFactory.Text("Does your organization house parents with children?");
            public static Activity GetAcceptsServiceAnimals = MessageFactory.Text("Does your organization house people who have service animals?");
            public static Activity GetHasDemographicNotSober = MessageFactory.Text("Does your organization house people who are not sober?");
        }

        public static class Demographic
        {
            public static Activity GetHasDemographic = MessageFactory.Text("Does your organization work with a specific demographic?");
            public static Activity GetHasDemographicMen = MessageFactory.Text("Does your organization work with men?");
            public static Activity GetHasDemographicWomen = MessageFactory.Text("Does your organization work with women?");
            public static Activity GetHasDemographicNonbinary = MessageFactory.Text("Does your organization work with non-binary people?");
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

        public static class JobTrainingServices
        {
            public static Activity GetHasJobTraining = MessageFactory.Text("Does your organization offer job training services?");
            public static Activity GetJobTrainingPositions = MessageFactory.Text("How many TOTAL job training positions do you have?");
            public static Activity GetJobTrainingOpenings = MessageFactory.Text("How many OPEN job training positions do you have?");
            public static Activity GetHasJobTrainingWaitlist = MessageFactory.Text("Do you have a waitlist for your job training services?");
            public static Activity GetJobTrainingWaitlistPositions = MessageFactory.Text("How many people are on your waitlist?");

            public static string GetJobTrainingOpeningsErrorFormat(int totalOpenings)
            {
                return string.Format("Oops, the total number of job training openings ({0}) must be greater than the open positions.", totalOpenings);
            }
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
