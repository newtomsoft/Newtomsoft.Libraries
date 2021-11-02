using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.EntityFramework.Constants;
using Newtomsoft.EntityFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var repositoryStringWithPrefix = GetRepositoryName<T>(configuration);
            var repositoryString = repositoryStringWithPrefix.Split('_')[^1].ToUpperInvariant();
            var repositoryProvider = GetRepositoryProvider(repositoryString);
            switch (repositoryProvider)
            {
                case RepositoryProvider.INMEMORY:
                    services.AddDbContext<T>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Scoped);
                    break;
                case RepositoryProvider.SQLITE:
                    string path = GetLocalSqlite(configuration, repositoryStringWithPrefix);
                    services.AddDbContext<T>(options => options.UseSqlite(path));
                    break;
                case RepositoryProvider.SQLSERVER:
                    services.AddDbContext<T>(options => options.UseSqlServer(configuration.GetConnectionString(repositoryStringWithPrefix)), ServiceLifetime.Scoped);
                    break;
                case RepositoryProvider.MYSQL:
                    services.AddDbContext<T>(options => options.UseMySql(configuration.GetConnectionString(repositoryStringWithPrefix), CreateMySqlServerVersion()));
                    break;
                case RepositoryProvider.POSTGRESQL:
                    services.AddDbContext<T>(options => options.UseNpgsql(configuration.GetConnectionString(repositoryStringWithPrefix)), ServiceLifetime.Scoped);
                    break;
                case RepositoryProvider.ORACLE:
                    services.AddDbContext<T>(options => options.UseOracle(configuration.GetConnectionString(repositoryStringWithPrefix)),  ServiceLifetime.Scoped);
                    break;
                default:
                    throw new ArgumentException("No DbContext defined !");
            }
        }

        public static string GetRepositoryName<TContext>(IConfiguration configuration) => GetRepository(typeof(TContext).Name, configuration);

        public static string GetLocalSqlite(IConfiguration configuration, string repository)
            => AddPathToSqliteConectionString(Path.Combine(Directory.GetCurrentDirectory()), configuration.GetConnectionString(repository));

        /// <summary>
        /// Use in your IDesignTimeDbContextFactory implementation class
        /// </summary>
        public static T CreateDbContext(string adminRepositoryKeyPrefix = "Admin_", string runningEnvironment = "")
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
            var providerString = GetProvider(repository);
            var repositoryProvider = GetRepositoryProvider(providerString);
            var connectionString = GetConnectionString(configuration, repository, repositoryProvider);
            UseDatabase(optionBuilder, repositoryProvider, connectionString);
            return (T)Activator.CreateInstance(typeof(T), optionBuilder.Options);
        }

        private static RepositoryProvider GetRepositoryProvider(string provider)
        {
            var availableProvidersDic = Enum.GetValues(typeof(RepositoryProvider)).Cast<RepositoryProvider>().ToDictionary(t => t.ToString(), t => t);
            var availableProviders = availableProvidersDic.Select(element => element.Key).Aggregate((current, next) => $"{current}, {next}");

            if (!availableProvidersDic.TryGetValue(provider, out var repositoryProvider))
                throw new RepositoryProviderException($"{provider} is not supported. available providers are : {availableProviders}");
            return repositoryProvider;
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
            if (runningEnvironment == ".") runningEnvironment = "InvalidEnvironment";
            IConfigurationBuilder builder;
            if (IsDotNetEFCommandWitchCallProgram())
                builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
                .AddJsonFile($"appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{runningEnvironment}json", optional: true);
            else
                builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{runningEnvironment}json", optional: true);
            return builder.Build();
        }

        private static string GetConnectionString(IConfigurationRoot configuration, string repository, RepositoryProvider provider)
        {
            string connectionString = configuration.GetConnectionString(repository);
            if (string.IsNullOrEmpty(connectionString) && provider != RepositoryProvider.INMEMORY)
                throw new ConnectionStringException("connectionString is not define !");
            if (provider == RepositoryProvider.SQLITE)
                connectionString = AddPathToSqliteConectionString(Directory.GetCurrentDirectory(), connectionString);
            Console.WriteLine($"connectionString is : {connectionString}");
            return connectionString;
        }

        private static string GetRepository(string dbContextName, IConfiguration configuration, string adminRepositoryKeyPrefix = "")
        {
            string defaultRepository = RepositoryProvider.SQLITE.ToString();
            var repository = configuration.GetValue<string>(NewtomsoftConfiguration.REPOSITORY_KEY);
            if (string.IsNullOrEmpty(repository))
                repository = configuration.GetValue($"{NewtomsoftConfiguration.REPOSITORY_KEY}:{dbContextName}", defaultRepository);

            return adminRepositoryKeyPrefix + repository;
        }

        private static void UseDatabase(DbContextOptionsBuilder<T> optionBuilder, RepositoryProvider provider, string connectionString)
        {
            var useProviders = new Dictionary<RepositoryProvider, Action<string>>
            {
                { RepositoryProvider.INMEMORY, connectionString => throw new ConnectionStringException($"You don't need to use Connection string in {RepositoryProvider.INMEMORY} mode") },
                { RepositoryProvider.SQLITE, connectionString => optionBuilder.UseSqlite(connectionString) },
                { RepositoryProvider.SQLSERVER, connectionString => optionBuilder.UseSqlServer(connectionString) },
                { RepositoryProvider.POSTGRESQL, connectionString => optionBuilder.UseNpgsql(connectionString) },
                { RepositoryProvider.MYSQL, connectionString => optionBuilder.UseMySql(connectionString, CreateMySqlServerVersion()) },
                { RepositoryProvider.ORACLE, connectionString => optionBuilder.UseOracle(connectionString) },
            };
            useProviders[provider].Invoke(connectionString);
        }

        private static string AddPathToSqliteConectionString(string path, string connectionString)
        {
            string[] splitConnectionString = connectionString.Split("#PATH#");
            return splitConnectionString[0] + Path.Combine(path, splitConnectionString[1]);
        }

        private static bool IsDotNetEFCommandWitchCallProgram() => Assembly.GetEntryAssembly().GetName().Name == "ef";

        private static string GetProvider(string repository) => repository.Split('_')[^1].ToUpperInvariant();

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

        private static MySqlServerVersion CreateMySqlServerVersion() => new(new Version(8, 0, 27));

        #endregion
    }
}
