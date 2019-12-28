using EntityModel;
using EntityModel.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.ApiInterface;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Shared
{
    public static class Helpers
    {
        /// <summary>
        /// Twilio is currently the only supported interface and it does not support
        /// "\r\n" from Environment.NewLine. We need to use "\n" instead.
        /// </summary>
        public static string NewLine { get { return "\n"; } }

        /// <summary>
        /// Gets a user token from the turn context.
        /// This will vary based on the channel the message is rec.
        /// </summary>
        public static string GetUserToken(ITurnContext turnContext)
        {
            switch (turnContext.Activity.ChannelId)
            {
                case Channels.Emulator: return turnContext.Activity.From.Id;
                case Channels.Sms: return PhoneNumberHelpers.Standardize(turnContext.Activity.From.Id);
                default: Debug.Fail("Missing channel type"); return string.Empty;
            }
        }        

        /// <summary>
        /// Creates a derived type of a type.
        /// </summary>
        public static T CreateSubType<T>(T subtype)
        {
            return (T)Activator.CreateInstance(subtype.GetType());
        }

        /// <summary>
        /// Gets the derived types of a type.
        /// </summary>
        public static List<T> GetSubtypes<T>()
        {
            List<T> results = new List<T>();

            var subtypes = Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)));

            foreach (Type subtype in subtypes)
            {
                results.Add((T)Activator.CreateInstance(subtype));
            }

            return results;
        }

        /// <summary>
        /// Gets the derived types of a type that meet a condition.
        /// </summary>
        public static List<T> GetSubtypes<T>(Func<T, bool> where = null, Func<T, object> orderBy = null)
        {
            var results = GetSubtypes<T>();

            if (where != null)
            {
                results = results.Where(where).ToList();
            }

            if (orderBy != null)
            {
                results = results.OrderBy(orderBy).ToList();
            }

            return results;
        }

        /// <summary>
        /// Gets the derived types of <see cref="ServiceData"/>.
        /// </summary>
        public static List<ServiceData> GetServiceDataTypes()
        {
            return GetSubtypes<ServiceData>();
        }

        /// <summary>
        /// Gets the service data for the given service type.
        /// </summary>
        public static ServiceData GetServiceDataTypeByServiceType(ServiceType serviceType)
        {
            return GetSubtypes<ServiceData>().FirstOrDefault(s => serviceType == s.ServiceType());
        }

        /// <summary>
        /// Gets the services for each of the given service types.
        /// </summary>
        public static List<ServiceData> GetServiceDataTypeByServiceType(IEnumerable<ServiceType> serviceTypes)
        {
            return GetSubtypes<ServiceData>(where: s => serviceTypes.Contains(s.ServiceType()), orderBy: s => s.ServiceTypeName())
                .ToList();
        }        

        /// <summary>
        /// Gets the first service whose type name matches the given type name.
        /// </summary>
        public static ServiceData GetServiceDataTypeByName(string typeName)
        {
            return GetSubtypes<ServiceData>(where: s => s.ServiceTypeName() == typeName, orderBy: s => s.ServiceTypeName())
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the type names for each of the given service types.
        /// </summary>
        public static List<string> GetServiceTypeNames(IEnumerable<ServiceType> serviceTypes, bool toLower = false)
        {
            return GetServiceDataTypeByServiceType(serviceTypes)
                .Select(s => toLower ? s.ServiceTypeName().ToLower() : s.ServiceTypeName())
                .ToList();
        }

        /// <summary>
        /// Gets the string from a list of services types.
        /// </summary>
        public static string GetServicesString(ServiceFlags serviceFlags)
        {
            List<string> names = new List<string>();

            foreach (var flag in ServiceFlagsHelpers.SplitFlags(serviceFlags))
            {
                var dataType = ServiceFlagToDataType(flag);
                var name = dataType.ServiceTypeName();

                var category = dataType.ServiceCategories().FirstOrDefault(c => c.ServiceFlags.HasFlag(flag));
                if (category != null && category.Name != name)
                {
                    name = $"{category.Name} {name}";
                }

                names.Add(name);
            }

            return GetServicesString(names);
        }

        /// <summary>
        /// Gets the string from a list of data types.
        /// </summary>
        public static string GetServicesString(List<ServiceData> types)
        {
            return GetServicesString(types.Select(t => t.ServiceTypeName()).ToList());
        }

        /// <summary>
        /// Gets the string from a list of service names.
        /// </summary>
        public static string GetServicesString(List<string> names)
        {
            if (names.Count() == 0)
            {
                return string.Empty;
            }

            if (names.Count() == 1)
            {
                return names[0].ToLower();
            }

            if (names.Count() == 2)
            {
                return $"{names[0].ToLower()} and {names[1].ToLower()}";
            }
            else
            {
                string result = string.Empty;

                for (int i = 0; i < names.Count; ++i)
                {
                    var separator = (i == names.Count - 1) ? ", and " : (!string.IsNullOrEmpty(result) ? ", " : string.Empty);
                    result += separator + names[i].ToLower();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the data type that handles a service flag.
        /// </summary>
        public static ServiceData ServiceFlagToDataType(ServiceFlags serviceFlag)
        {
            return GetServiceDataTypes().FirstOrDefault(t => t.ServiceCategories().Any(c => c.ServiceFlags.HasFlag(serviceFlag)));
        }

        public static async Task<string> GetLatestUpdateString(IApiInterface api, string organizationId)
        {
            var result = string.Empty;

            foreach (var dataType in GetServiceDataTypes())
            {
                var data = await api.GetLatestServiceData(organizationId, dataType);
                if (data != null)
                {
                    // Add a newline if there is already some text.
                    result += string.IsNullOrEmpty(result) ? string.Empty : NewLine;
                    result += data.ToString();
                }
            }

            return result;
        }

        /// <summary>
        /// Looks up a position from a location.
        /// </summary>
        public static async Task<LocationPosition> LocationToPosition(IConfiguration configuration, string location)
        {
            var url = string.Format(configuration.MapsSearchUrlFormat(), configuration.MapsSubscriptionKey(), location);
            var response = await new HttpClient().GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<LocationApiResponse>(responseContent);
            if (data == null && data.Summary.NumResults == 0)
            {
                return null;
            }

            // Return the first city in the results.
            return data.Results.FirstOrDefault(r => r.EntityType == EntityType.Municipality)?.Position;
        }

        public static string DayToGreeting(DateTime dateTime)
        {
            // Special greetings for holidays.
            if (dateTime.Month == 1 && dateTime.Day == 1)
            {
                return "Happy New Year";
            }
            else if (dateTime.Month == 4 && dateTime.Day == 1)
            {
                return "Happy April Fools' Day - don't get fooled today";
            }
            else if (dateTime.Month == 5 && dateTime.Day == 4)
            {
                return "Happy Star Wars Day - May the 4th be with you";
            }
            else if (dateTime.Month == 10 && dateTime.Day == 31)
            {
                return "Happy Halloween";
            }
            else if (dateTime.Month == 12 && dateTime.Day == 25)
            {
                return "Merry Christmas";
            }
            else
            {
                // Generic greetings for the day of the week.
                var localDay = DayFlagsHelpers.FromDateTime(dateTime);
                switch (localDay)
                {
                    case DayFlags.Monday: return "Hope you had a great weekend";
                    default: return $"Happy {localDay.ToString()}";
                }
            }
        }

        public static void LogInfo(ILogger log, string text)
        {
            if (log != null)
            {
                log.LogInformation(text);
            }
        }

        public static void LogException(ILogger log, Exception exception)
        {
            if (log != null)
            {
                log.LogError(exception, exception.Message);
            }
        }
    }
}
