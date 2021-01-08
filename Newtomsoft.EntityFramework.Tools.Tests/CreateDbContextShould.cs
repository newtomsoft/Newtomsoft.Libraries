using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Microsoft.Extensions.Hosting;

namespace Newtomsoft.EntityFramework.Tools.Tests
{
    public class Startup
    {
        public void ConfigureHost(IHostBuilder hostBuilder)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            hostBuilder.ConfigureHostConfiguration(builder => builder.AddConfiguration(config));
        }
    }


    public class CreateDbContextShould
    {
        public readonly IConfiguration _config;

        public CreateDbContextShould(IConfiguration config)
        {
            _config = config;
        }


        [Fact]
        public void Test2()
        {
            Environment.SetEnvironmentVariable("REPOSITORY", "Sqlite");
            Environment.SetEnvironmentVariable("MyEnvVar", "Dev");
            var dbContext = EntityFrameworkTools<DefaultDbContext>.CreateDbContext();
            Assert.NotNull(dbContext);
        }
    }
}
