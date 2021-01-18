using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Newtomsoft.Configuration
{
    public static class NewtomsoftConfiguration
    {
        public const string CONSOLE_OUTPUT = "Console";
        public const string JSONCONSOLE_OUTPUT = "JsonConsole";
        public const string DEBUG_OUTPUT = "Debug";
        public const string DEFAULT_ENVIRONMENT_DEVELOPMENT_VALUE = "Development";

        public static IConfigurationRoot GetConfiguration(string environement, string customConfigFileName = null)
        {
            if (string.IsNullOrEmpty(environement)) environement = DEFAULT_ENVIRONMENT_DEVELOPMENT_VALUE;

            var builder = new ConfigurationBuilder()
                        .AddJsonFile($"sharesettings.{environement}.json", optional: true)
                        .AddJsonFile($"appsettings.{environement}.json", optional: true)
                        .AddJsonFile("sharesettings.json", optional: true)
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
