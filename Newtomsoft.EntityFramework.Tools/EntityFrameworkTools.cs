using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Newtomsoft.EntityFramework.Tools
{
    public static class EntityFrameworkTools<T> where T : DbContext
    {
        /// <summary>
        /// repository mode
        /// </summary>
        private const string SQLITE = "Sqlite";
        private const string SQLSERVER = "SqlServer";
        private const string POSTGRESQL = "PostgreSql";
        private const string MYSQL = "MySql";
        private const string IN_MEMORY = "InMemory";

        /// <summary>
        /// default .NET_ENVIRONMENT env 
        /// </summary>
        private const string DEVELOPMENT = "Development";

        /// <summary>
        /// Use in your Startup ConfigureServices (for asp.net)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            string repository = GetEnvironmentVariable("REPOSITORY", IN_MEMORY, "PERSISTENCE");
            if (repository == IN_MEMORY)
                services.AddDbContext<T>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Scoped);
            else if (repository == SQLITE)
                services.AddDbContext<T>(options => options.UseSqlite(AddPathToSqliteConectionString(Path.Combine(Directory.GetCurrentDirectory()), configuration.GetConnectionString(SQLITE))));
            else if (repository == SQLSERVER)
                services.AddDbContext<T>(options => options.UseSqlServer(configuration.GetConnectionString(SQLSERVER)), ServiceLifetime.Scoped);
            else if (repository == MYSQL)
                services.AddDbContext<T>(options => options.UseMySQL(configuration.GetConnectionString(MYSQL)), ServiceLifetime.Scoped);
            else
                throw new ArgumentException("No DbContext defined !");
        }

        /// <summary>
        /// Use in your CreateDbContext class
        /// </summary>
        /// <returns></returns>
        public static T CreateDbContext()
        {
            DbContextOptionsBuilder<T> optionBuilder = new DbContextOptionsBuilder<T>();

            string repository = GetEnvironmentVariable("REPOSITORY", IN_MEMORY, "PERSISTENCE");

            string aspdotnetEnv = GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", string.Empty);
            string dotnetEnv = GetEnvironmentVariable("DOTNET_ENVIRONMENT", string.Empty);
            string goodDotNetEnvironment = GetGoodDotNetEnvironment(aspdotnetEnv, dotnetEnv);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "..");
            var builder = new ConfigurationBuilder()
                          .SetBasePath(path)
                          .AddJsonFile($"appsettings.{goodDotNetEnvironment}.json", optional: true)
                          .AddJsonFile($"sharesettings.{goodDotNetEnvironment}.json", optional: true);
            string connectionString = builder.Build().GetConnectionString(repository);
            if (repository == SQLITE)
                connectionString = AddPathToSqliteConectionString(path, connectionString);
            Console.WriteLine($"connectionString is : {connectionString}");
            UseDatabase(optionBuilder, repository, connectionString);
            return (T)Activator.CreateInstance(typeof(T), optionBuilder.Options);
        }


        private static string GetGoodDotNetEnvironment(string aspdotnetEnv, string dotnetEnv)
        {
            string env;
            if (string.IsNullOrEmpty(dotnetEnv) && string.IsNullOrEmpty(aspdotnetEnv))
            {
                env = DEVELOPMENT;
                Console.WriteLine($"ASPNETCORE_ENVIRONMENT and DOTNET_ENVIRONMENT are not defined Set to {env}");
            }
            else if (!string.IsNullOrEmpty(dotnetEnv) && !string.IsNullOrEmpty(aspdotnetEnv))
            {
                env = dotnetEnv;
                Console.WriteLine($"ASPNETCORE_ENVIRONMENT and DOTNET_ENVIRONMENT are twice defined. using DOTNET_ENVIRONMENT at {env}");
            }
            else if (!string.IsNullOrEmpty(dotnetEnv))
            {
                env = dotnetEnv;
                Console.WriteLine($"DOTNET_ENVIRONMENT only is defined. Using it at {env}");
            }
            else
            {
                env = aspdotnetEnv;
                Console.WriteLine($"ASPNETCORE_ENVIRONMENT only is defined. Using it at {env}");
            }
            return env;
        }

        private static string GetEnvironmentVariable(string EnvironmentName, string defaultEnvironmentValue, string oldEnvironmentName = "")
        {
            var oldEnvironmentValue = string.Empty;
            if (!string.IsNullOrEmpty(oldEnvironmentName))
            {
                oldEnvironmentValue = Environment.GetEnvironmentVariable(oldEnvironmentName);
                if (!string.IsNullOrEmpty(oldEnvironmentValue))
                    Console.WriteLine($"'{oldEnvironmentName}' is not longer use. Please use '{EnvironmentName}' ");
            }
            var environmentValue = Environment.GetEnvironmentVariable(EnvironmentName);
            if (string.IsNullOrEmpty(environmentValue))
            {
                if (!string.IsNullOrEmpty(oldEnvironmentValue))
                    environmentValue = oldEnvironmentValue;
                else
                    environmentValue = defaultEnvironmentValue;
                Console.WriteLine($"'{EnvironmentName}' is not define. Define to {environmentValue}");
            }
            else
            {
                Console.WriteLine($"'{EnvironmentName}' is {environmentValue}");
            }
            return environmentValue;
        }

        private static void UseDatabase(DbContextOptionsBuilder<T> optionBuilder, string persistence, string connectionString)
        {
            if (persistence == SQLSERVER)
                optionBuilder.UseSqlServer(connectionString);
            else if (persistence == POSTGRESQL)
                optionBuilder.UseNpgsql(connectionString);
            else if (persistence == SQLITE)
                optionBuilder.UseSqlite(connectionString);
            else if (persistence == MYSQL)
                optionBuilder.UseMySQL(connectionString);
        }

        private static string AddPathToSqliteConectionString(string path, string connectionString)
        {
            string[] splitConnectionString = connectionString.Split("#PATH#");
            return splitConnectionString[0] + Path.Combine(path, splitConnectionString[1]);
        }
    }
}
