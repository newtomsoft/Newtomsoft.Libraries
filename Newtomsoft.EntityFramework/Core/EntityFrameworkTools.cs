using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.EntityFramework.Constants;
using Newtomsoft.EntityFramework.Exceptions;
using Oracle.EntityFrameworkCore.Infrastructure;
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
                case RepositoryProvider.Inmemory:
                    services.AddDbContext<T>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
                    break;
                case RepositoryProvider.Sqlite:
                    var path = GetLocalSqlite(configuration, repositoryStringWithPrefix);
                    services.AddDbContext<T>(options => options.UseSqlite(path));
                    break;
                case RepositoryProvider.SqlServer:
                    services.AddDbContext<T>(options => options.UseSqlServer(configuration.GetConnectionString(repositoryStringWithPrefix)));
                    break;
                case RepositoryProvider.MySql:
                    services.AddDbContext<T>(options => options.UseMySql(configuration.GetConnectionString(repositoryStringWithPrefix), CreateMySqlServerVersion()));
                    break;
                case RepositoryProvider.PostgreSql:
                    services.AddDbContext<T>(options => options.UseNpgsql(configuration.GetConnectionString(repositoryStringWithPrefix)));
                    break;
                case RepositoryProvider.Oracle:
                    services.AddDbContext<T>(options => options.UseOracle(configuration.GetConnectionString(repositoryStringWithPrefix), OracleVersion11));
                    break;
                default:
                    throw new ArgumentException("No DbContext defined !");
            }
        }

        private static string GetRepositoryName<TContext>(IConfiguration configuration) => GetRepository(typeof(TContext).Name, configuration);

        private static string GetLocalSqlite(IConfiguration configuration, string repository)
            => AddPathToSqliteConnectionString(Path.Combine(Directory.GetCurrentDirectory()), configuration.GetConnectionString(repository));

        /// <summary>
        /// Use in your IDesignTimeDbContextFactory implementation class
        /// </summary>
        public static T CreateDbContext(string adminRepositoryKeyPrefix = "Admin_", string runningEnvironment = "")
        {
            if (string.IsNullOrEmpty(runningEnvironment)) runningEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var optionBuilder = new DbContextOptionsBuilder<T>();
            var dbContextName = typeof(T).Name;
            runningEnvironment = GetRunningEnvironmentFromDbContextName(dbContextName) ?? runningEnvironment;
            Console.WriteLine(string.IsNullOrEmpty(runningEnvironment)
                ? $"No runningEnvironment found. Using generic settings file."
                : $"runningEnvironment is : {runningEnvironment}");
            var configuration = GetConfiguration(runningEnvironment);
            var repository = GetRepository(dbContextName, configuration, adminRepositoryKeyPrefix);
            Console.WriteLine($"using is : {dbContextName} with {repository}");
            var providerString = GetProvider(repository);
            var repositoryProvider = GetRepositoryProvider(providerString);
            var connectionString = GetConnectionString(configuration, repository, repositoryProvider);
            UseDatabase(optionBuilder, repositoryProvider, connectionString);
            return (T)Activator.CreateInstance(typeof(T), optionBuilder.Options);
        }

        private static RepositoryProvider GetRepositoryProvider(string provider)
        {
            var availableProvidersDic = Enum.GetValues(typeof(RepositoryProvider)).Cast<RepositoryProvider>().ToDictionary(t => t.ToString().ToUpperInvariant(), t => t);
            var availableProviders = availableProvidersDic.Select(element => element.Key).Aggregate((current, next) => $"{current}, {next}");

            if (!availableProvidersDic.TryGetValue(provider, out var repositoryProvider))
                throw new RepositoryProviderException($"{provider} is not supported. available providers are : {availableProviders}");
            return repositoryProvider;
        }

        public static string GetEnvironmentVariable(string environmentName, string defaultEnvironmentValue)
        {
            var environmentValue = Environment.GetEnvironmentVariable(environmentName, EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(environmentValue))
            {
                environmentValue = defaultEnvironmentValue;
                Console.WriteLine($"'{environmentName}' is not define. Define to {environmentValue}");
            }
            else
            {
                Console.WriteLine($"'{environmentName}' is {environmentValue}");
            }
            return environmentValue;
        }

        #region Private methods
        private static IConfigurationRoot GetConfiguration(string runningEnvironment)
        {
            runningEnvironment += ".";
            if (runningEnvironment == ".") runningEnvironment = "InvalidEnvironment";
            IConfigurationBuilder builder;
            if (IsDotNetEfCommandWitchCallProgram())
                builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory())?.FullName)
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
            var connectionString = configuration.GetConnectionString(repository);
            if (string.IsNullOrEmpty(connectionString) && provider != RepositoryProvider.Inmemory)
                throw new ConnectionStringException("connectionString is not define !");
            if (provider == RepositoryProvider.Sqlite)
                connectionString = AddPathToSqliteConnectionString(Directory.GetCurrentDirectory(), connectionString);
            Console.WriteLine($"connectionString is : {connectionString}");
            return connectionString;
        }

        private static string GetRepository(string dbContextName, IConfiguration configuration, string adminRepositoryKeyPrefix = "")
        {
            var defaultRepository = RepositoryProvider.Sqlite.ToString();
            var repository = configuration.GetValue<string>(NewtomsoftConfiguration.RepositoryKey);
            if (string.IsNullOrEmpty(repository))
                repository = configuration.GetValue($"{NewtomsoftConfiguration.RepositoryKey}:{dbContextName}", defaultRepository);

            return adminRepositoryKeyPrefix + repository;
        }

        private static void UseDatabase(DbContextOptionsBuilder<T> optionBuilder, RepositoryProvider provider, string connectionString)
        {
            var useProviders = new Dictionary<RepositoryProvider, Action>
            {
                { RepositoryProvider.Inmemory, () => throw new ConnectionStringException($"You don't need to use Connection string in {RepositoryProvider.Inmemory} mode") },
                { RepositoryProvider.Sqlite, () => optionBuilder.UseSqlite(connectionString) },
                { RepositoryProvider.SqlServer, () => optionBuilder.UseSqlServer(connectionString) },
                { RepositoryProvider.PostgreSql, () => optionBuilder.UseNpgsql(connectionString) },
                { RepositoryProvider.MySql, () => optionBuilder.UseMySql(connectionString, CreateMySqlServerVersion()) },
                { RepositoryProvider.Oracle, () => optionBuilder.UseOracle(connectionString, OracleVersion11) },
            };
            useProviders[provider].Invoke();
        }

        private static string AddPathToSqliteConnectionString(string path, string connectionString)
        {
            var splitConnectionString = connectionString.Split("#PATH#");
            return splitConnectionString[0] + Path.Combine(path, splitConnectionString[1]);
        }

        private static bool IsDotNetEfCommandWitchCallProgram() => Assembly.GetEntryAssembly()!.GetName().Name == "ef";

        private static string GetProvider(string repository) => repository.Split('_')[^1].ToUpperInvariant();

        private static string GetRunningEnvironmentFromDbContextName(string dbContextName)
        {
            if (!dbContextName.EndsWith("Environment"))
            {
                Console.WriteLine($"no environment evaluate by DbContext");
                return null;
            }
            var runningEnvironment = dbContextName.Split('_')[^1].Split("Environment")[0];
            Console.WriteLine($"Environment evaluate by DbContext to {runningEnvironment}");
            return runningEnvironment;
        }

        private static MySqlServerVersion CreateMySqlServerVersion() => new(new Version(8, 0, 22));
        private static Action<OracleDbContextOptionsBuilder> OracleVersion11 => x => x.UseOracleSQLCompatibility("11");

        #endregion
    }
}
