using Microsoft.EntityFrameworkCore.Design;
using Newtomsoft.EntityFramework.Core;
using Newtomsoft.EntityFramework.Tests.DbContexts;

namespace Newtomsoft.EntityFramework.Tests.DesignTimeDbContextFactory
{
    public class GoodDesignTimeDbContextFactoryDevelopment : IDesignTimeDbContextFactory<GoodDbContext_Development_>
    {
        public GoodDbContext_Development_ CreateDbContext(string[] args) => EntityFrameworkTools<GoodDbContext_Development_>.CreateDbContext();
    }

    public class GoodDesignTimeDbContextFactoryStaging : IDesignTimeDbContextFactory<GoodDbContext_Staging_>
    {
        public GoodDbContext_Staging_ CreateDbContext(string[] args) => EntityFrameworkTools<GoodDbContext_Staging_>.CreateDbContext();
    }
}