using Microsoft.EntityFrameworkCore.Design;
using Newtomsoft.EntityFramework.Tools.Core;
using Newtomsoft.EntityFramework.Tools.Tests.DbContexts;

namespace Newtomsoft.EntityFramework.Tools.Tests.DesignTimeDbContextFactory
{
    public class GoodDesignTimeDbContextFactory : IDesignTimeDbContextFactory<GoodDbContext>
    {
        public GoodDbContext CreateDbContext(string[] args) => EntityFrameworkTools<GoodDbContext>.CreateDbContext();
    }
}