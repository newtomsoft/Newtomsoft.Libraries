using Newtomsoft.EntityFramework.Core;
using Newtomsoft.EntityFramework.Exceptions;
using Newtomsoft.EntityFramework.Tests.DbContexts;
using Shouldly;
using System.IO;
using Xunit;

namespace Newtomsoft.EntityFramework.Tests
{
    public class CreateDbContextShould
    {
        private const string DEVELOPMENT_ENVIRONMENT = "Development";
        private const string STAGING_ENVIRONMENT = "Staging";

        [Fact]
        public void CreateDbContextWhenRepositoryAndConnectionStringAreGoodInSettingsFile()
        {
            var testDbContext = EntityFrameworkTools<GoodDbContext_Development>.CreateDbContext("", DEVELOPMENT_ENVIRONMENT);
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowConnectionStringExceptionWhenInMemory()
        {
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<InMemoryDbContext>.CreateDbContext("", DEVELOPMENT_ENVIRONMENT));
        }

        [Fact]
        public void CreateDbContextWhenOtherGoodEnvironmentAndSettingsFileExist()
        {
            var testDbContext = EntityFrameworkTools<GoodDbContext_Staging>.CreateDbContext("", STAGING_ENVIRONMENT);
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowConnectionStringExceptionWhenEnvironmentIsBad()
        {
            Should.Throw<FileNotFoundException>(() => EntityFrameworkTools<GoodDbContextBase>.CreateDbContext("", "UnknowEnvironement"));
        }

        [Fact]
        public void ThrowRepositoryProviderExceptionWhenRepositoryAreBadInSettingsFile()
        {
            Should.Throw<RepositoryProviderException>(() => EntityFrameworkTools<BadProviderDbContext>.CreateDbContext("", DEVELOPMENT_ENVIRONMENT));
        }

        [Fact]
        public void ThrowConnectionStringException()
        {
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<NoConnectionStringForThisDbContext>.CreateDbContext("", DEVELOPMENT_ENVIRONMENT));
        }

        [Fact]
        public void CreateDbContextWhenRepositoryAndConnectionStringWithAdminPrefixAreGoodInSettingsFile()
        {
            var testDbContext = EntityFrameworkTools<GoodDbContext_Development>.CreateDbContext("Admin_", DEVELOPMENT_ENVIRONMENT);
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void ThrowWhenRepositoryPrefixAreBadInSettingsFile()
        {
            Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<GoodDbContext_Development>.CreateDbContext("BadPrefix_", DEVELOPMENT_ENVIRONMENT));
        }

        [Fact]
        public void CreateDbContextWhenGoodMySql()
        {
            var testDbContext = EntityFrameworkTools<MySqlDbContext>.CreateDbContext("", DEVELOPMENT_ENVIRONMENT);
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void CreateDbContextWhenGoodMySqlInGenericSettingsFile()
        {
            var testDbContext = EntityFrameworkTools<MySqlDbContext>.CreateDbContext();
            testDbContext.ShouldNotBeNull();
            testDbContext.Cities.ShouldNotBeNull();
            testDbContext.Countries.ShouldNotBeNull();
        }
    }
}
