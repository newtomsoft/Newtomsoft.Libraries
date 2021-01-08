using Microsoft.EntityFrameworkCore.Design;

namespace Newtomsoft.EntityFramework.Tools.Demo.Repository
{
    public class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
    {
        public DefaultDbContext CreateDbContext(string[] args) => EntityFrameworkTools<DefaultDbContext>.CreateDbContext();
    }
}