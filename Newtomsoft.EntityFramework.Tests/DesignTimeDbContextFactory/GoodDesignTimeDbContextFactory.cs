using Microsoft.EntityFrameworkCore.Design;
using Newtomsoft.EntityFramework.Core;
using Newtomsoft.EntityFramework.Tests.DbContexts;

namespace Newtomsoft.EntityFramework.Tests.DesignTimeDbContextFactory
{
    public class GoodDesignTimeDbContextFactoryDevelopment : IDesignTimeDbContextFactory<GoodDbContext_Development>
    {
        public GoodDbContext_Development CreateDbContext(string[] args) => EntityFrameworkTools<GoodDbContext_Development>.CreateDbContext();
    }

    public class GoodDesignTimeDbContextFactoryStaging : IDesignTimeDbContextFactory<GoodDbContext_Staging>
    {
        public GoodDbContext_Staging CreateDbContext(string[] args) => EntityFrameworkTools<GoodDbContext_Staging>.CreateDbContext();
    }
}