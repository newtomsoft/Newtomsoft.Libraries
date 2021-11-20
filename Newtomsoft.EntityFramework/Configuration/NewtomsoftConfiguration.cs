using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Newtomsoft.EntityFramework.Configuration
{
    public static class NewtomsoftConfiguration
    {
        private const string DefaultEnvironmentDevelopmentValue = "Development";

        public const string ConsoleOutput = "Console";
        public const string JsonconsoleOutput = "JsonConsole";
        public const string DebugOutput = "Debug";

        public static IConfigurationRoot GetConfiguration(string environment = null, string customConfigFileName = null)
        {
            if (string.IsNullOrEmpty(environment)) environment = DefaultEnvironmentDevelopmentValue;

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
