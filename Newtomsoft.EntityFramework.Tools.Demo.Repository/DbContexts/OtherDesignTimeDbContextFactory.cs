using Microsoft.EntityFrameworkCore.Design;
using Newtomsoft.EntityFramework.Tools.Core;

namespace Newtomsoft.EntityFramework.Tools.Demo.Repository.DbContexts
{
    public class OtherDesignTimeDbContextFactory : IDesignTimeDbContextFactory<OtherDbContext>
    {
        public OtherDbContext CreateDbContext(string[] args) => EntityFrameworkTools<OtherDbContext>.CreateDbContext();
    }
}