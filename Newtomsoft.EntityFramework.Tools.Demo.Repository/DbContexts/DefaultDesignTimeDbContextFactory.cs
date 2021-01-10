using Microsoft.EntityFrameworkCore.Design;
using Newtomsoft.EntityFramework.Tools.Core;

namespace Newtomsoft.EntityFramework.Tools.Demo.Repository.DbContexts
{
    public class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
    {
        public DefaultDbContext CreateDbContext(string[] args) => EntityFrameworkTools<DefaultDbContext>.CreateDbContext();
    }
}