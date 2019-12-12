using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Models;
using System;
using System.Collections;
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
        /// Gets a user token from the turn context.
        /// This will vary based on the channel the message is rec.
        /// </summary>
        public static string GetUserToken(ITurnContext turnContext)
        {
            switch (turnContext.Activity.ChannelId)
            {
                case Channels.Emulator: return turnContext.Activity.From.Id;
                case Channels.Sms: return PhoneNumber.Standardize(turnContext.Activity.From.Id);
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
        /// Gets the service for the given service type.
        /// </summary>
        public static ServiceData GetServiceByType(ServiceType serviceType)
        {
            return GetSubtypes<ServiceData>().FirstOrDefault(s => serviceType == s.ServiceType());
        }

        /// <summary>
        /// Gets the services for each of the given service types.
        /// </summary>
        public static List<ServiceData> GetServicesByType(IEnumerable<ServiceType> serviceTypes)
        {
            return GetSubtypes<ServiceData>(where: s => serviceTypes.Contains(s.ServiceType()), orderBy: s => s.ServiceTypeName())
                .ToList();
        }

        /// <summary>
        /// Gets the type names for each of the given service types.
        /// </summary>
        public static List<string> GetServiceTypeNames(IEnumerable<ServiceType> serviceTypes, bool toLower = false)
        {
            return GetServicesByType(serviceTypes)
                .Select(s => toLower ? s.ServiceTypeName().ToLower() : s.ServiceTypeName())
                .ToList();
        }

        /// <summary>
        /// Gets the first service whose type name matches the given type name.
        /// </summary>
        public static ServiceData GetServiceTypeByName(string typeName)
        {
            return GetSubtypes<ServiceData>(where: s => s.ServiceTypeName() == typeName, orderBy: s => s.ServiceTypeName())
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the string from a list of services types.
        /// </summary>
        public static string GetServicesString(ServiceFlags serviceFlags)
        {
            List<string> names = new List<string>();

            foreach (var flag in SplitServiceFlags(serviceFlags))
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

        public static IEnumerable<ServiceFlags> GetServiceFlags()
        {
            return Enum.GetValues(typeof(ServiceFlags)).OfType<ServiceFlags>().Where(f => f != ServiceFlags.None);
        }

        /// <summary>
        /// Splits service flags into their individual flags.
        /// </summary>
        public static IEnumerable<ServiceFlags> SplitServiceFlags(ServiceFlags flags)
        {
            foreach (ServiceFlags value in GetServiceFlags())
            {
                if (flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Gets the data type that handles a service flag.
        /// </summary>
        public static ServiceData ServiceFlagToDataType(ServiceFlags serviceFlag)
        {
            return GetServiceDataTypes().FirstOrDefault(t => t.ServiceCategories().Any(c => c.ServiceFlags.HasFlag(serviceFlag)));
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
    }
}
