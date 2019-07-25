
using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;
using Shared.Models;

namespace Shared
{
    public static class Helpers
    {
        /// <summary>
        /// Gets a user ID from the turn context.
        /// </summary>
        public static string UserId(ITurnContext context)
        {
            return "9a0f1731-45ae-e911-a97f-000d3a30da4f";
            //return context.Activity.From.Id;
        }

        /// <summary>
        /// Gets the table name for a service.
        /// </summary>
        public static string GetServiceTableName(ServiceType serviceType)
        {
            switch (serviceType)
            {
                case ServiceType.Housing: return HousingData.TABLE_NAME;
                case ServiceType.Advocacy: return "TODO";
                case ServiceType.MentalHealth: return MentalHealthData.TABLE_NAME;
                case ServiceType.SubstanceUse: return "TODO";
                case ServiceType.JobTraining: return JobTrainingData.TABLE_NAME;
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Gets the primary key for a service.
        /// </summary>
        public static string GetServicePrimaryKey(ServiceType serviceType)
        {
            switch (serviceType)
            {
                case ServiceType.Housing: return HousingData.PRIMARY_KEY;
                case ServiceType.Advocacy: return "TODO";
                case ServiceType.MentalHealth: return MentalHealthData.PRIMARY_KEY;
                case ServiceType.SubstanceUse: return "TODO";
                case ServiceType.JobTraining: return JobTrainingData.PRIMARY_KEY;
                default: return string.Empty;
            }
        }
    }
}
