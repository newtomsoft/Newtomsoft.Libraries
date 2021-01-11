using Newtomsoft.EntityFramework.Constants;
using Newtomsoft.EntityFramework.Core;
using Newtomsoft.EntityFramework.Exceptions;
using Newtomsoft.EntityFramework.Tests.DbContexts;
using Shouldly;
using System;
using Xunit;

namespace Newtomsoft.EntityFramework.Tests
{
    public class CreateDbContextShould
    {
        [Fact]
        public void CreateDbContextWhenRepositoryAndConnectionStringAreGoodInSettingsFile()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, "Development", EnvironmentVariableTarget.User);
            var testDbContext = EntityFrameworkTools<GoodDbContext>.CreateDbContext();
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowConnectionStringExceptionWhenInMemory()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, "Development", EnvironmentVariableTarget.User);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<InMemoryDbContext>.CreateDbContext());
        }

        //[Fact]
        //public void ThrowConnectionZZZZZZZZ()
        //{
        //    Environment.SetEnvironmentVariable(EntityFrameworkTools<GoodDbContext>.DOTNET_ENVIRONMENT, "uvygvzsedyvuzey", EnvironmentVariableTarget.User);
        //    var env = Environment.GetEnvironmentVariable(EntityFrameworkTools<GoodDbContext>.DOTNET_ENVIRONMENT, EnvironmentVariableTarget.User);
        //    var envReturn = DebugClass.GetEnvironmentVariable();
        //    env.ShouldBe(envReturn);
        //}

        [Fact]
        public void OtherGoodEnvironment()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, "Staging", EnvironmentVariableTarget.User);
            var testDbContext = EntityFrameworkTools<GoodDbContext>.CreateDbContext();
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowConnectionStringExceptionWhenEnvironmentIsBad()
        {
            Environment.SetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, "UnknownEnvironment!", EnvironmentVariableTarget.User);
            var env = Environment.GetEnvironmentVariable(NewtomsoftEnvironment.DOTNET_ENVIRONMENT_KEY, EnvironmentVariableTarget.User);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<GoodDbContext>.CreateDbContext());
        }

        [Fact]
        public void ThrowRepositoryProviderExceptionWhenRepositoryAreBadInSettingsFile()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development", EnvironmentVariableTarget.User);
            Should.Throw<RepositoryProviderException>(() => EntityFrameworkTools<BadProviderDbContext>.CreateDbContext());
        }

        [Fact]
        public void ThrowConnectionStringException()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development", EnvironmentVariableTarget.User);
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<NoConnectionStringForThisDbContext>.CreateDbContext());
        }
    }
}
