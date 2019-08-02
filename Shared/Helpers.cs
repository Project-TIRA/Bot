
using EntityModel;
using Microsoft.Bot.Builder;
using Shared.ApiInterface;
using System.Diagnostics;

namespace Shared
{
    public static class Helpers
    {
        /// <summary>
        /// Gets a user token from the turn context.
        /// This will vary based on the channel the bot is running in.
        /// i.e. Phone number for SMS, GUID for emulator
        /// </summary>
        public static string GetUserToken(ITurnContext context)
        {
            return "+17605004495";
            //return context.Activity.From.Id;
        }

        /// <summary>
        /// Gets the table name for a service.
        /// </summary>
        public static ServiceType GetServiceType<T>() where T : ServiceModelBase
        {
            var type = typeof(T);

            if (type == typeof(CaseManagementData))
            {
                return ServiceType.CaseManagement;
            }
            if (type == typeof(HousingData))
            {
                return ServiceType.Housing;
            }
            else if (type == typeof(JobTrainingData))
            {
                return ServiceType.JobTraining;
            }
            else if (type == typeof(MentalHealthData))
            {
                return ServiceType.MentalHealth;
            }
            else if (type == typeof(SubstanceUseData))
            {
                return ServiceType.SubstanceUse;
            }

            Debug.Assert(false, "Service type not recognized");
            return ServiceType.Invalid;
        }

        /// <summary>
        /// Gets the table name for a service.
        /// </summary>
        public static string GetServiceTableName(ServiceType serviceType)
        {
            switch (serviceType)
            {
                case ServiceType.Housing: return HousingData.TABLE_NAME;
                case ServiceType.CaseManagement: return "TODO";
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
                case ServiceType.CaseManagement: return "TODO";
                case ServiceType.MentalHealth: return MentalHealthData.PRIMARY_KEY;
                case ServiceType.SubstanceUse: return "TODO";
                case ServiceType.JobTraining: return JobTrainingData.PRIMARY_KEY;
                default: return string.Empty;
            }
        }
    }
}
