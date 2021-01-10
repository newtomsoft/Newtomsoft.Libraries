using Newtomsoft.EntityFramework.Tools.Core;
using Newtomsoft.EntityFramework.Tools.Exceptions;
using Newtomsoft.EntityFramework.Tools.Tests.DbContexts;
using Shouldly;
using System;
using Xunit;

namespace Newtomsoft.EntityFramework.Tools.Tests
{
    public class CreateDbContextShould
    {
        [Fact]
        public void CreateDbContextWhenRepositoryAndConnectionStringAreGoodInSettingsFile()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development", EnvironmentVariableTarget.User);
            var testDbContext = EntityFrameworkTools<GoodDbContext>.CreateDbContext();
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
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
            Environment.SetEnvironmentVariable(EntityFrameworkTools<GoodDbContext>.DOTNET_ENVIRONMENT_KEY, "Staging", EnvironmentVariableTarget.User);
            var testDbContext = EntityFrameworkTools<GoodDbContext>.CreateDbContext();
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowConnectionStringExceptionWhenEnvironmentIsBad()
        {
            Environment.SetEnvironmentVariable(EntityFrameworkTools<GoodDbContext>.DOTNET_ENVIRONMENT_KEY, "UnknownEnvironment!", EnvironmentVariableTarget.User);
            var env = Environment.GetEnvironmentVariable(EntityFrameworkTools<GoodDbContext>.DOTNET_ENVIRONMENT_KEY, EnvironmentVariableTarget.User);
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
