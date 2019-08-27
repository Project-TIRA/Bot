﻿using Microsoft.Extensions.Configuration;
using System;

namespace Shared
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// The name of the app setting for the environment.
        /// </summary>
        private const string EnvironmentSettingName = "Environment";

        /// <summary>
        /// The name of the app setting for the database connection string.
        /// </summary>
        private const string DbModelConnectionStringSettingName = "DbModel";

        /// <summary>
        /// The name of the app setting for the MicrosoftAppId.
        /// </summary>
        private const string MicrosoftAppIdSettingName = "MicrosoftAppId";

        /// <summary>
        /// The name of the app setting for the MicrosoftAppPassword.
        /// </summary>
        private const string MicrosoftAppPasswordSettingName = "MicrosoftAppPassword";

        /// <summary>
        /// The name of the setting that contains the CosmosDB endpoint.
        /// </summary>
        private const string CosmosEndpointSettingName = "CosmosDb:Endpoint";

        /// <summary>
        /// The name of the setting that contains the CosmosDB key.
        /// </summary>
        private const string CosmosKeySettingName = "CosmosDb:Key";

        /// <summary>
        /// The name of the setting that contains the CosmosDB database.
        /// </summary>
        private const string CosmosDatabaseSettingName = "CosmosDb:Database";

        /// <summary>
        /// The name of the setting that contains the CosmosDB collection.
        /// </summary>
        private const string CosmosCollectionSettingName = "CosmosDb:Collection";

        /// <summary>
        /// The name of the setting that contains the CosmosDB collection.
        /// </summary>
        private const string SnapshotCollectorConfigurationSettingName = "SnapshotCollectorConfiguration";

        /// <summary>
        /// The name of the setting that contains the channel ID for tests.
        /// </summary>
        private const string TestChannelSettingName = "TestChannel";

        public static string Environment(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(EnvironmentSettingName);
        }

        public static string MicrosoftAppId(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(MicrosoftAppIdSettingName);
        }

        public static string MicrosoftAppPassword(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(MicrosoftAppPasswordSettingName);
        }

        public static string DbModelConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString(DbModelConnectionStringSettingName);
        }

        public static Uri CosmosEndpoint(this IConfiguration configuration)
        {
            return new Uri(configuration.GetValue<string>(CosmosEndpointSettingName));
        }

        public static string CosmosKey(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosKeySettingName);
        }

        public static string CosmosDatabase(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosDatabaseSettingName);
        }

        public static string CosmosCollection(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosCollectionSettingName);
        }

        public static IConfigurationSection SnapshotCollectorConfiguration(this IConfiguration configuration)
        {
            return configuration.GetSection(SnapshotCollectorConfigurationSettingName);
        }

        public static string TestChannel(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(TestChannelSettingName);
        }

        public static bool IsProduction(this IConfiguration configuration)
        {
            return string.Equals(configuration.Environment(), "Production", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}