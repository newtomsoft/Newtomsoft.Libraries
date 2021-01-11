using Microsoft.EntityFrameworkCore.Design;
using Newtomsoft.EntityFramework.Core;
using Newtomsoft.EntityFramework.Tests.DbContexts;

namespace Newtomsoft.EntityFramework.Tests.DesignTimeDbContextFactory
{
    public class GoodDesignTimeDbContextFactory : IDesignTimeDbContextFactory<GoodDbContext>
    {
        public GoodDbContext CreateDbContext(string[] args) => EntityFrameworkTools<GoodDbContext>.CreateDbContext();
    }
}