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
        private const string Environment = "NewtomsoftEntityFrameworkTestEnvironment";
        private const string Development = "Development";
        private ServiceCollection _services;
        private IConfigurationRoot _configuration;

        #region private methods
        private void CreateServices()
        {
            System.Environment.SetEnvironmentVariable(Environment, Development, EnvironmentVariableTarget.User);
            _configuration = GetConfiguration();
            _services = new ServiceCollection();
            _services.AddSingleton<IConfiguration>(_configuration);
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.Development.json", optional: true);

            return builder.Build();
        }

        private static void AddContryToDatabase(GoodDbContext_Development dbContext)
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
            EntityFrameworkTools<InMemoryDbContext>.AddDbContext(_services, _configuration);
            var provider = _services.BuildServiceProvider();
            var dbContext = provider.GetService<NoConnectionStringForThisDbContext>();
            dbContext.ShouldBeNull();
        }

        [Fact]
        public void GetDbContextWhenInMemory()
        {
            CreateServices();
            EntityFrameworkTools<InMemoryDbContext>.AddDbContext(_services, _configuration);
            var provider = _services.BuildServiceProvider();
            var dbContext = provider.GetService<InMemoryDbContext>();
            dbContext.ShouldNotBeNull();
            dbContext.Cities.ShouldNotBeNull();
            dbContext.Countries.ShouldNotBeNull();
        }

        [Fact]
        public void GetDbContextWhenDataBaseExist()
        {
            CreateServices();
            EntityFrameworkTools<GoodDbContext_Development>.AddDbContext(_services, _configuration);
            var provider = _services.BuildServiceProvider();
            var dbContext = provider.GetService<GoodDbContext_Development>();
            dbContext.ShouldNotBeNull();
            dbContext.Cities.ShouldNotBeNull();
            dbContext.Countries.ShouldNotBeNull();
        }

        /// <summary>
        /// To pass theses tests successfully please copy 
        /// </summary>
        [Fact]
        public void GetDbContextWithNoCityWhenDataBaseExistAndCitiesNoFilled()
        {
            CreateServices();
            EntityFrameworkTools<GoodDbContext_Development>.AddDbContext(_services, _configuration);
            var provider = _services.BuildServiceProvider();
            var dbContext = provider.GetService<GoodDbContext_Development>();
            dbContext.ShouldNotBeNull();
            dbContext.Cities.ShouldNotBeNull();
            dbContext.Cities.ToList().Count.ShouldBe(0);
        }

        [Fact]
        public void GetDbContextWith1CountryWhenDataBaseExistAnd1CountryAdd()
        {
            CreateServices();
            EntityFrameworkTools<GoodDbContext_Development>.AddDbContext(_services, _configuration);
            var provider = _services.BuildServiceProvider();
            var dbContext = provider.GetService<GoodDbContext_Development>();
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
