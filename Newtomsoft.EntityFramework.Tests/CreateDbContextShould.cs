using Newtomsoft.EntityFramework.Constants;
using Newtomsoft.EntityFramework.Core;
using Newtomsoft.EntityFramework.Exceptions;
using Newtomsoft.EntityFramework.Tests.DbContexts;
using Shouldly;
using System;
using Xunit;

namespace Newtomsoft.EntityFramework.Tests
{
    /// <summary>
    /// Run these tests as administrator for successfully
    /// </summary>
    public class CreateDbContextShould
    {
        private const string DEVELOPMENT_DOTNET_ENVIRONMENT = "Development";
        private const string STAGING_DOTNET_ENVIRONMENT = "Staging";
        private const string UNKNOWN_DOTNET_ENVIRONMENT = "UnknownEnvironment!";

        [Fact]
        public void CreateDbContextWhenRepositoryAndConnectionStringAreGoodInSettingsFile()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.Machine);
            var testDbContext = EntityFrameworkTools<GoodDbContext>.CreateDbContext();
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowConnectionStringExceptionWhenInMemory()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.Machine);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<InMemoryDbContext>.CreateDbContext());
        }

        [Fact]
        public void OtherGoodEnvironment()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, STAGING_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.Machine);
            var testDbContext = EntityFrameworkTools<GoodDbContext>.CreateDbContext();
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowConnectionStringExceptionWhenEnvironmentIsBad()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, UNKNOWN_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.Machine);
            var env = Environment.GetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, EnvironmentVariableTarget.Machine);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<GoodDbContext>.CreateDbContext());
        }

        [Fact]
        public void ThrowRepositoryProviderExceptionWhenRepositoryAreBadInSettingsFile()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.Machine);
            Should.Throw<RepositoryProviderException>(() => EntityFrameworkTools<BadProviderDbContext>.CreateDbContext());
        }

        [Fact]
        public void ThrowConnectionStringException()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.Machine);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<NoConnectionStringForThisDbContext>.CreateDbContext());
        }
    }
}
