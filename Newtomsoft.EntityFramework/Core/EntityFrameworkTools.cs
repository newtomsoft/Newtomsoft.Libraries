using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.EntityFramework.Constants;
using Newtomsoft.EntityFramework.Exceptions;
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
            string repository = GetRepositoryName<T>(configuration);
            switch (GetProvider(repository))
            {
                case RepositoryProvider.IN_MEMORY:
                    services.AddDbContext<T>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Scoped);
                    break;
                case RepositoryProvider.SQLITE:
                    string path = GetLocalSqlite(configuration, repository);
                    services.AddDbContext<T>(options => options.UseSqlite(path));
                    break;
                case RepositoryProvider.SQLSERVER:
                    services.AddDbContext<T>(options => options.UseSqlServer(configuration.GetConnectionString(repository)), ServiceLifetime.Scoped);
                    break;
                case RepositoryProvider.MYSQL:
                    services.AddDbContext<T>(options => options.UseMySql(configuration.GetConnectionString(repository), CreateMySqlServerVersion()));
                    break;
                case RepositoryProvider.POSTGRESQL:
                    services.AddDbContext<T>(options => options.UseNpgsql(configuration.GetConnectionString(repository)), ServiceLifetime.Scoped);
                    break;
                default:
                    throw new ArgumentException("No DbContext defined !");
            }
        }

        public static string GetRepositoryName<TContext>(IConfiguration configuration)
        {
            var dbContextName = typeof(TContext).Name;
            var repository = GetRepository(dbContextName, configuration);
            return repository;
        }

        public static string GetLocalSqlite(IConfiguration configuration, string repository)
            => AddPathToSqliteConectionString(Path.Combine(Directory.GetCurrentDirectory()), configuration.GetConnectionString(repository));

        /// <summary>
        /// Use in your IDesignTimeDbContextFactory implementation class
        /// </summary>
        public static T CreateDbContext(string adminRepositoryKeyPrefix = "", string runningEnvironment = "")
        {
            if (string.IsNullOrEmpty(runningEnvironment)) runningEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var optionBuilder = new DbContextOptionsBuilder<T>();
            var dbContextName = typeof(T).Name;
            runningEnvironment = GetRunningEnvironementFromDbContextName(dbContextName) ?? runningEnvironment;
            if (string.IsNullOrEmpty(runningEnvironment))
                Console.WriteLine($"No runningEnvironment found. Using generic settings file.");
            else
                Console.WriteLine($"runningEnvironment is : {runningEnvironment}");
            var configuration = GetConfiguration(runningEnvironment);
            string repository = GetRepository(dbContextName, configuration, adminRepositoryKeyPrefix);
            Console.WriteLine($"using is : {dbContextName} with {repository}");
            var provider = GetProvider(repository);
            var connectionString = GetConnectionString(configuration, repository, provider);
            UseDatabase(optionBuilder, provider, connectionString);
            return (T)Activator.CreateInstance(typeof(T), optionBuilder.Options);
        }

        public static string GetEnvironmentVariable(string EnvironmentName, string defaultEnvironmentValue)
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

        #region Private methods
        private static IConfigurationRoot GetConfiguration(string runningEnvironment)
        {
            runningEnvironment += ".";
            if (runningEnvironment == ".") runningEnvironment = string.Empty;
            IConfigurationBuilder builder;
            if (IsDotNetEFCommandWitchCallProgram())
            {
                builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
                .AddJsonFile($"appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{runningEnvironment}json", optional: false);
            }
            else
                builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{runningEnvironment}json", optional: false);
            return builder.Build();
        }

        private static string GetConnectionString(IConfigurationRoot configuration, string repository, string provider)
        {
            string connectionString = configuration.GetConnectionString(repository);
            if (string.IsNullOrEmpty(connectionString) && provider != RepositoryProvider.IN_MEMORY)
                throw new ConnectionStringException("connectionString is not define !");
            if (provider == RepositoryProvider.SQLITE)
                connectionString = AddPathToSqliteConectionString(Directory.GetCurrentDirectory(), connectionString);
            Console.WriteLine($"connectionString is : {connectionString}");
            return connectionString;
        }

        private static string GetRepository(string dbContextName, IConfiguration configuration, string adminRepositoryKeyPrefix = "")
        {
            var repository = configuration.GetValue<string>(NewtomsoftConfiguration.REPOSITORY_KEY);
            if (string.IsNullOrEmpty(repository))
                repository = configuration.GetValue($"{NewtomsoftConfiguration.REPOSITORY_KEY}:{dbContextName}", RepositoryProvider.SQLSERVER);

            return adminRepositoryKeyPrefix + repository;
        }

        private static void UseDatabase(DbContextOptionsBuilder<T> optionBuilder, string provider, string connectionString)
        {
            var useProviders = new Dictionary<string, Action<string>>
            {
                { RepositoryProvider.SQLSERVER, connectionString => optionBuilder.UseSqlServer(connectionString) },
                { RepositoryProvider.POSTGRESQL, connectionString => optionBuilder.UseNpgsql(connectionString) },
                { RepositoryProvider.SQLITE, connectionString => optionBuilder.UseSqlite(connectionString) },
                { RepositoryProvider.MYSQL, connectionString => optionBuilder.UseMySql(connectionString, CreateMySqlServerVersion()) },
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

        private static bool IsDotNetEFCommandWitchCallProgram() => Assembly.GetEntryAssembly().GetName().Name == "ef";

        private static string GetProvider(string repository) => repository.Split('_')[^1];

        private static string GetRunningEnvironementFromDbContextName(string dbContextName)
        {
            if (!dbContextName.EndsWith("Environment"))
            {
                Console.WriteLine($"no environment evaluate by DbContext");
                return null;
            }
            string runningEnvironement = dbContextName.Split('_')[^1].Split("Environment")[0];
            Console.WriteLine($"Environement evaluate by DbContext to {runningEnvironement}");
            return runningEnvironement;
        }

        private static MySqlServerVersion CreateMySqlServerVersion() => new(new Version(8, 0, 22));

        #endregion
    }
}
