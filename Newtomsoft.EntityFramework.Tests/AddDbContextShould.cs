using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.EntityFramework.Core;
using Newtomsoft.EntityFramework.Tests.DbContexts;
using Newtomsoft.EntityFramework.Tests.Models;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Newtomsoft.EntityFramework.Tests
{
    public class AddDbContextShould : IClassFixture<FixtureEnvironment>
    {
        private const string TestCountryName = "France";
        private ServiceCollection Services;
        private IConfigurationRoot Configuration;

        #region private methods
        private void CreateServices()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development", EnvironmentVariableTarget.User);
            Configuration = GetConfiguration();
            Services = new ServiceCollection();
            Services.AddSingleton<IConfiguration>(Configuration);
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                        .AddJsonFile("sharesettings.Development.json", optional: true)
                        .AddJsonFile("appsettings.Development.json", optional: true);

            return builder.Build();
        }

        private static void AddContryToDatabase(GoodDbContext dbContext)
        {
            if (dbContext.Countries.Where(c => c.Name == TestCountryName).FirstOrDefault() == null)
            {
                dbContext.Countries.Add(new CountryModel(TestCountryName, false));
                dbContext.SaveChanges();
            }
        }
        #endregion

        [Fact]
        public void DontGetDbContextWhenDataBaseDontExist()
        {
            CreateServices();
            EntityFrameworkTools<InMemoryDbContext>.AddDbContext(Services, Configuration);
            var provider = Services.BuildServiceProvider();
            var dbContext = provider.GetService<NoConnectionStringForThisDbContext>();
            dbContext.ShouldBeNull();
        }

        [Fact]
        public void GetDbContextWhenInMemory()
        {
            CreateServices();
            EntityFrameworkTools<InMemoryDbContext>.AddDbContext(Services, Configuration);
            var provider = Services.BuildServiceProvider();
            var dbContext = provider.GetService<InMemoryDbContext>();
            dbContext.ShouldNotBeNull();
            dbContext.Cities.ShouldNotBeNull();
            dbContext.Countries.ShouldNotBeNull();
        }

        /// <summary>
        /// To pass this test execute dotnet ef migrations add Init -c GoodDbContext --no-build
        /// And dotnet ef database update -c GoodDbContext --no-build in console at tests directory
        /// </summary>
        [Fact]
        public void GetDbContextWhenDataBaseExist()
        {
            CreateServices();
            EntityFrameworkTools<GoodDbContext>.AddDbContext(Services, Configuration);
            var provider = Services.BuildServiceProvider();
            var dbContext = provider.GetService<GoodDbContext>();
            dbContext.ShouldNotBeNull();
            dbContext.Cities.ShouldNotBeNull();
            dbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void GetDbContextWithNoCityWhenDataBaseExistAndCitiesNoFilled()
        {
            CreateServices();
            EntityFrameworkTools<GoodDbContext>.AddDbContext(Services, Configuration);
            var provider = Services.BuildServiceProvider();
            var dbContext = provider.GetService<GoodDbContext>();
            dbContext.ShouldNotBeNull();
            dbContext.Cities.ShouldNotBeNull();
            dbContext.Cities.ToList().Count.ShouldBe(0);
        }

        [Fact]
        public void GetDbContextWith1ContryWhenDataBaseExistAnd1ContryAdd()
        {
            CreateServices();
            EntityFrameworkTools<GoodDbContext>.AddDbContext(Services, Configuration);
            var provider = Services.BuildServiceProvider();
            var dbContext = provider.GetService<GoodDbContext>();
            dbContext.ShouldNotBeNull();
            AddContryToDatabase(dbContext);
            dbContext.Countries.ShouldNotBeNull();
            var countries = dbContext.Countries.ToList();
            countries.Count.ShouldBe(1);
            countries[0].Name.ShouldBe(TestCountryName);
            countries[0].IsDemocracy.ShouldBeFalse();
        }
    }
}
