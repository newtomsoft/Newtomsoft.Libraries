using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.EntityFramework.Constants;
using Newtomsoft.EntityFramework.Exceptions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
        public static void AddDbContext(IServiceCollection services, IConfiguration configuration)
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
                    services.AddDbContextPool<T>(options => options.UseMySql(configuration.GetConnectionString(repository), CreateMySqlServerVersion(), mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)));
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
        public static T CreateDbContext(string adminRepositoryKeyPrefix = "")
        {
            DbContextOptionsBuilder<T> optionBuilder = new DbContextOptionsBuilder<T>();
            var dbContextName = typeof(T).Name;
            string runningEnvironment = GetRunningEnvironment();
            var configuration = GetConfiguration(runningEnvironment);
            string repository = GetRepository(dbContextName, configuration, adminRepositoryKeyPrefix);
            Console.WriteLine($"using is : {dbContextName} with {repository}");
            var provider = GetProvider(repository);
            var connectionString = GetConnectionString(configuration, repository, provider);
            if (string.IsNullOrEmpty(connectionString) && provider != RepositoryProvider.IN_MEMORY)
                throw new ConnectionStringException("connectionString is dot define !");
            UseDatabase(optionBuilder, provider, connectionString);
            return (T)Activator.CreateInstance(typeof(T), optionBuilder.Options);
        }

        #region Private methods
        private static IConfigurationRoot GetConfiguration(string runningEnvironment)
        {
            IConfigurationBuilder builder;
            if (IsDotNetEFCommand())
            {
                builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
                .AddJsonFile($"appsettings.{runningEnvironment}.json", optional: true)
                .AddJsonFile($"sharesettings.{runningEnvironment}.json", optional: true);
            }
            else
                builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{runningEnvironment}.json", optional: true)
                .AddJsonFile($"sharesettings.{runningEnvironment}.json", optional: true);
            return builder.Build();
        }

        private static string GetConnectionString(IConfigurationRoot configuration, string repository, string provider)
        {
            string connectionString = configuration.GetConnectionString(repository);
            if (provider == RepositoryProvider.SQLITE)
                connectionString = AddPathToSqliteConectionString(Directory.GetCurrentDirectory(), connectionString);
            Console.WriteLine($"connectionString is : {connectionString}");
            return connectionString;
        }

        private static string GetRepository(string dbContextName, IConfiguration configuration, string repositoryKeyPrefix = "")
        {
            var repository = configuration.GetValue<string>(NewtomsoftEnvironment.REPOSITORY_KEY);
            if (string.IsNullOrEmpty(repository))
                repository = configuration.GetValue($"{NewtomsoftEnvironment.REPOSITORY_KEY}:{dbContextName}", RepositoryProvider.SQLSERVER);

            return repositoryKeyPrefix + repository;
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
            var environmentValue = Environment.GetEnvironmentVariable(EnvironmentName, EnvironmentVariableTarget.User);
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
                { RepositoryProvider.MYSQL, connectionString => optionBuilder.UseMySql(connectionString, CreateMySqlServerVersion(), mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)) },
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

        private static bool IsDotNetEFCommand() => Assembly.GetEntryAssembly().GetName().Name == "ef";

        private static string GetProvider(string repository) => repository.Split('_')[^1];

        private static MySqlServerVersion CreateMySqlServerVersion() => new MySqlServerVersion(new Version(8, 0, 22));
        #endregion
    }
}
