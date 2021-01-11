using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.EntityFramework.Exceptions;
using Newtomsoft.EntityFramework.Constants;
using System;
using System.Collections.Generic;
using System.IO;

namespace Newtomsoft.EntityFramework.Core
{
    public static class EntityFrameworkTools<T> where T : DbContext
    {
        /// <summary>
        /// Use in your Startup class, ConfigureServices method (for asp.net)
        /// or in your App class, Application_Startup method (for .net)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDbContext(IServiceCollection services, IConfigurationRoot configuration)
        {
            var dbContextName = typeof(T).Name;
            var repository = GetRepository(dbContextName, configuration);
            var provider = repository.Split('_')[^1];

            switch (provider)
            {
                case RepositoryProvider.IN_MEMORY:
                    services.AddDbContext<T>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Scoped);
                    break;

                case RepositoryProvider.SQLITE:
                    var path = AddPathToSqliteConectionString(Path.Combine(Directory.GetCurrentDirectory()), configuration.GetConnectionString(repository));
                    services.AddDbContext<T>(options => options.UseSqlite(path));
                    break;

                case RepositoryProvider.SQLSERVER:
                    services.AddDbContext<T>(options => options.UseSqlServer(configuration.GetConnectionString(repository)), ServiceLifetime.Scoped);
                    break;

                case RepositoryProvider.MYSQL:
                    services.AddDbContext<T>(options => options.UseMySQL(configuration.GetConnectionString(repository)), ServiceLifetime.Scoped);
                    break;

                case RepositoryProvider.POSTGRESQL:
                    services.AddDbContext<T>(options => options.UseNpgsql(configuration.GetConnectionString(repository)), ServiceLifetime.Scoped);
                    break;

                default:
                    throw new ArgumentException("No DbContext defined !");
            }
        }

        /// <summary>
        /// Use in your IDesignTimeDbContextFactory implementation class
        /// </summary>
        public static T CreateDbContext()
        {
            DbContextOptionsBuilder<T> optionBuilder = new DbContextOptionsBuilder<T>();
            var dbContextName = typeof(T).Name;
            string runningEnvironment = GetRunningEnvironment();
            string currentPath = Directory.GetCurrentDirectory();
            string parentPath = Path.Combine(currentPath, "..");
            var builder = new ConfigurationBuilder()
                .SetBasePath(parentPath) // used by command dotnet ef for migrations management
                .AddJsonFile($"sharesettings.{runningEnvironment}.json", optional: true)
                .SetBasePath(currentPath) // used by running mode
                .AddJsonFile($"appsettings.{runningEnvironment}.json", optional: true)
                .AddJsonFile($"sharesettings.{runningEnvironment}.json", optional: true);
            var configuration = builder.Build();
            string repository = GetRepository(dbContextName, configuration);
            Console.WriteLine($"using is : {dbContextName} with {repository}");
            string provider = GetProvider(repository);
            string connectionString = GetConnectionString(configuration, repository, provider);
            UseDatabase(optionBuilder, provider, connectionString);
            return (T)Activator.CreateInstance(typeof(T), optionBuilder.Options);
        }

        #region Private methods
        private static string GetConnectionString(IConfigurationRoot configuration, string repository, string provider)
        {
            string connectionString = configuration.GetConnectionString(repository);
            if (string.IsNullOrEmpty(connectionString) && provider != RepositoryProvider.IN_MEMORY)
                throw new ConnectionStringException("connectionString is dot define !");
            if (provider == RepositoryProvider.SQLITE)
                connectionString = AddPathToSqliteConectionString(Directory.GetCurrentDirectory(), connectionString);
            Console.WriteLine($"connectionString is : {connectionString}");
            return connectionString;
        }

        private static string GetRepository(string dbContextName, IConfigurationRoot configuration)
        {
            var repository = configuration.GetValue<string>(NewtomsoftEnvironment.REPOSITORY_KEY);
            if (string.IsNullOrEmpty(repository))
                repository = configuration.GetValue($"{NewtomsoftEnvironment.REPOSITORY_KEY}:{dbContextName}", RepositoryProvider.SQLSERVER);

            return repository;
        }

        private static string GetRunningEnvironment()
        {
            string aspdotnetEnvironment = GetEnvironmentVariable(NewtomsoftEnvironment.ASPNETCORE_ENVIRONMENT_KEY, string.Empty);
            string dotnetEnvironment = GetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, string.Empty);
            return GetRunningEnvironment(aspdotnetEnvironment, dotnetEnvironment);
        }

        private static string GetRunningEnvironment(string aspdotnetEnv, string dotnetEnv)
        {
            string env;
            if (string.IsNullOrEmpty(dotnetEnv) && string.IsNullOrEmpty(aspdotnetEnv))
            {
                env = NewtomsoftEnvironment.DEVELOPMENT_RUNNING;
                Console.WriteLine($"{NewtomsoftEnvironment.ASPNETCORE_ENVIRONMENT_KEY} and {NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY} are not defined Set to {env}");
            }
            else if (!string.IsNullOrEmpty(dotnetEnv) && !string.IsNullOrEmpty(aspdotnetEnv))
            {
                env = dotnetEnv;
                Console.WriteLine($"{NewtomsoftEnvironment.ASPNETCORE_ENVIRONMENT_KEY} and {NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY} are twice defined. using {NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY} at {env}");
            }
            else if (!string.IsNullOrEmpty(dotnetEnv))
            {
                env = dotnetEnv;
                Console.WriteLine($"{NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY} only is defined. Using it at {env}");
            }
            else
            {
                env = aspdotnetEnv;
                Console.WriteLine($"{NewtomsoftEnvironment.ASPNETCORE_ENVIRONMENT_KEY} only is defined. Using it at {env}");
            }
            return env;
        }

        private static string GetEnvironmentVariable(string EnvironmentName, string defaultEnvironmentValue)
        {
            var environmentValue = System.Environment.GetEnvironmentVariable(EnvironmentName, EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(environmentValue))
            {
                environmentValue = defaultEnvironmentValue;
                Console.WriteLine($"'{EnvironmentName}' is not define. Define to {environmentValue}");
            }
            else
            {
                Console.WriteLine($"'{EnvironmentName}' is {environmentValue}");
            }
            return environmentValue;
        }

        private static void UseDatabase(DbContextOptionsBuilder<T> optionBuilder, string provider, string connectionString)
        {
            var useProviders = new Dictionary<string, Action<string>>
            {
                { RepositoryProvider.SQLSERVER, connectionString => optionBuilder.UseSqlServer(connectionString) },
                { RepositoryProvider.POSTGRESQL, connectionString => optionBuilder.UseNpgsql(connectionString) },
                { RepositoryProvider.SQLITE, connectionString => optionBuilder.UseSqlite(connectionString) },
                { RepositoryProvider.MYSQL, connectionString => optionBuilder.UseMySQL(connectionString) },
                { RepositoryProvider.IN_MEMORY, connectionString => throw new ConnectionStringException($"You don't need to use Connection string in {RepositoryProvider.IN_MEMORY} mode") }
            };
            if (!useProviders.TryGetValue(provider, out var useProvider))
                throw new RepositoryProviderException($"{provider} is not in the settings file !");

            useProvider.Invoke(connectionString);
        }

        private static string AddPathToSqliteConectionString(string path, string connectionString)
        {
            string[] splitConnectionString = connectionString.Split("#PATH#");
            return splitConnectionString[0] + Path.Combine(path, splitConnectionString[1]);
        }

        private static string GetProvider(string repository) => repository.Split('_')[^1];
        #endregion
    }
}
