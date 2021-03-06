﻿using Microsoft.Extensions.Configuration;

namespace DAS.DigitalEngagement.Framework.Infrastructure.Configuration
{
    static class ConfigurationHelper
    {
        public static string GetEnvironmentName(this IConfiguration configuration)
        {
            return configuration.GetConnectionStringOrSetting("EnvironmentName");
        }

        public static string GetAppName(this IConfiguration configuration)
        {
            var appName = configuration.GetConnectionStringOrSetting("APPSETTING_AppName");

            if (appName == null)
            {
                appName = configuration.GetConnectionStringOrSetting("AppName");
            }
            return appName;
        }

        public static string GetAzureStorageConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionStringOrSetting("AzureWebJobsStorage");
        }
    }
}
