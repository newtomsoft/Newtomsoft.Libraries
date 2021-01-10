using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.EntityFramework.Tools.Core;
using Newtomsoft.EntityFramework.Tools.Demo.Repository.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtomsoft.EntityFramework.Tools.Demo.Program
{
    public class Program
    {
        static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            EntityFrameworkTools<DefaultDbContext>.AddDbContext(services, configuration);

            var provider = services.BuildServiceProvider();

            var defaultDbContext = provider.GetService<DefaultDbContext>();

            //var contries = defaultDbContext.Countries;


        }




        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                        .AddJsonFile("sharesettings.Development.json", optional: true)
                        .AddJsonFile("appsettings.Development.json", optional: true);

            return builder.Build();
        }
    }
}