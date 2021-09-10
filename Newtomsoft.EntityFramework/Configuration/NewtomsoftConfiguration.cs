using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Newtomsoft.EntityFramework.Configuration
{
    public static class NewtomsoftConfiguration
    {
        public const string DEFAULT_ENVIRONMENT_DEVELOPMENT_VALUE = "Development";

        public const string CONSOLE_OUTPUT = "Console";
        public const string JSONCONSOLE_OUTPUT = "JsonConsole";
        public const string DEBUG_OUTPUT = "Debug";

        public static IConfigurationRoot GetConfiguration(string environment = null, string customConfigFileName = null)
        {
            if (string.IsNullOrEmpty(environment)) environment = DEFAULT_ENVIRONMENT_DEVELOPMENT_VALUE;

            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true);

            if (!string.IsNullOrEmpty(customConfigFileName))
                builder.AddJsonFile(customConfigFileName, optional: false);

            return builder.Build();
        }

        public static ILogger GetLogger<T>(params string[] values)
        {
            var loggerFactory = LoggerFactory.Create(builder => Array.ForEach(values, value => builder.AddCustom(value)));
            return loggerFactory.CreateLogger<T>();
        }
    }
}
