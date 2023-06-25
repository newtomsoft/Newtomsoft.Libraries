using Newtomsoft.EntityFramework.Core;
using Newtomsoft.EntityFramework.Exceptions;
using Newtomsoft.EntityFramework.Tests.DbContexts;
using Shouldly;
using Xunit;

namespace Newtomsoft.EntityFramework.Tests;

public class CreateDbContextShould
{
    private const string DevelopmentEnvironment = "Development";
    private const string StagingEnvironment = "Staging";

    [Fact]
    public void CreateDbContextWhenRepositoryAndConnectionStringAreGoodInSettingsFile()
    {
        var testDbContext = EntityFrameworkTools<GoodDbContext_Development>.CreateDbContext("", DevelopmentEnvironment);
        testDbContext.ShouldNotBeNull();
        testDbContext.Cities.ShouldNotBeNull();
        testDbContext.Countries.ShouldNotBeNull();
    }

    [Fact]
    public void ThrowConnectionStringExceptionWhenInMemory()
    {
        Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<InMemoryDbContext>.CreateDbContext("", DevelopmentEnvironment));
    }

    [Fact]
    public void CreateDbContextWhenOtherGoodEnvironmentAndSettingsFileExist()
    {
        var testDbContext = EntityFrameworkTools<GoodDbContext_Staging>.CreateDbContext("", StagingEnvironment);
        testDbContext.ShouldNotBeNull();
        testDbContext.Cities.ShouldNotBeNull();
        testDbContext.Countries.ShouldNotBeNull();
    }

    [Fact]
    public void ThrowConnectionStringExceptionWhenEnvironmentIsBad()
    {
        Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<GoodDbContextBase>.CreateDbContext("", "UnknownEnvironment"));
    }

    [Fact]
    public void ThrowRepositoryProviderExceptionWhenRepositoryAreBadInSettingsFile()
    {
        Should.Throw<RepositoryProviderException>(() => EntityFrameworkTools<BadProviderDbContext>.CreateDbContext("", DevelopmentEnvironment));
    }

    [Fact]
    public void ThrowRepositoryProviderException()
    {
        Should.Throw<RepositoryProviderException>(() => EntityFrameworkTools<NoConnectionStringForThisDbContext>.CreateDbContext("", DevelopmentEnvironment));
    }

    [Fact]
    public void CreateDbContextWhenRepositoryAndConnectionStringWithAdminPrefixAreGoodInSettingsFile()
    {
        var testDbContext = EntityFrameworkTools<GoodDbContext_Development>.CreateDbContext("Admin_", DevelopmentEnvironment);
        testDbContext.ShouldNotBeNull();
        testDbContext.Cities.ShouldNotBeNull();
        testDbContext.Countries.ShouldNotBeNull();
    }

    [Fact]
    public void ThrowWhenRepositoryPrefixAreBadInSettingsFile()
    {
        Should.Throw<ConnectionStringException>(() => EntityFrameworkTools<GoodDbContext_Development>.CreateDbContext("BadPrefix_", DevelopmentEnvironment));
    }

    [Fact]
    public void CreateDbContextWhenGoodMySql()
    {
        var testDbContext = EntityFrameworkTools<MySqlDbContext>.CreateDbContext("", DevelopmentEnvironment);
        testDbContext.ShouldNotBeNull();
        testDbContext.Cities.ShouldNotBeNull();
        testDbContext.Countries.ShouldNotBeNull();
    }

    [Fact]
    public void CreateDbContextWhenGoodPostgres()
    {
        var testDbContext = EntityFrameworkTools<PostgresDbContext>.CreateDbContext("", DevelopmentEnvironment);
        testDbContext.ShouldNotBeNull();
        testDbContext.Cities.ShouldNotBeNull();
        testDbContext.Countries.ShouldNotBeNull();
    }

    [Fact]
    public void CreateDbContextWhenGoodSqlServer()
    {
        var testDbContext = EntityFrameworkTools<SqlServerDbContext>.CreateDbContext("", DevelopmentEnvironment);
        testDbContext.ShouldNotBeNull();
        testDbContext.Cities.ShouldNotBeNull();
        testDbContext.Countries.ShouldNotBeNull();
    }

    [Fact]
    public void CreateDbContextWhenGoodOracle()
    {
        var testDbContext = EntityFrameworkTools<OracleDbContext>.CreateDbContext("", DevelopmentEnvironment);
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