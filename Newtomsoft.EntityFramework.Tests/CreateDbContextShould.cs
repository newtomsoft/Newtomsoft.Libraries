using Newtomsoft.EntityFramework.Constants;
using Newtomsoft.EntityFramework.Core;
using Newtomsoft.EntityFramework.Exceptions;
using Newtomsoft.EntityFramework.Tests.DbContexts;
using Shouldly;
using System;
using Xunit;

namespace Newtomsoft.EntityFramework.Tests
{
    public class CreateDbContextShould : IClassFixture<FixtureEnvironment>
    {
        private const string DEVELOPMENT_DOTNET_ENVIRONMENT = "Development";
        private const string STAGING_DOTNET_ENVIRONMENT = "Staging";
        private const string UNKNOWN_DOTNET_ENVIRONMENT = "UnknownEnvironment!";

        [Fact]
        public void CreateDbContextWhenRepositoryAndConnectionStringAreGoodInSettingsFile()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.User);
            var testDbContext = EntityFrameworkTools<GoodDbContext>.CreateDbContext();
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowConnectionStringExceptionWhenInMemory()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.User);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<InMemoryDbContext>.CreateDbContext());
        }

        [Fact]
        public void CreateDbContextWhenOtherGoodEnvironmentAndSettingsFileExist()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, STAGING_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.User);
            var testDbContext = EntityFrameworkTools<GoodDbContext>.CreateDbContext();
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowConnectionStringExceptionWhenEnvironmentIsBad()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, UNKNOWN_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.User);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<GoodDbContext>.CreateDbContext());
        }

        [Fact]
        public void ThrowRepositoryProviderExceptionWhenRepositoryAreBadInSettingsFile()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.User);
            Should.Throw<RepositoryProviderException>(() => EntityFrameworkTools<BadProviderDbContext>.CreateDbContext());
        }

        [Fact]
        public void ThrowConnectionStringException()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.User);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<NoConnectionStringForThisDbContext>.CreateDbContext());
        }

        [Fact]
        public void CreateDbContextWhenRepositoryAndConnectionStringWithAdminPrefixAreGoodInSettingsFile()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.User);
            var testDbContext = EntityFrameworkTools<GoodDbContext>.CreateDbContext("Admin_");
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }
        [Fact]
        public void ThorwWhenRepositoryPrefixAreBadInSettingsFile()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, DEVELOPMENT_DOTNET_ENVIRONMENT, EnvironmentVariableTarget.User);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<GoodDbContext>.CreateDbContext("BadPrefix_"));
        }
    }
}
