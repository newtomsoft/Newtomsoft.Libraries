using Microsoft.EntityFrameworkCore;

namespace Newtomsoft.EntityFramework.Tests.DbContexts
{
    public class GoodDbContext_Development_ : GoodDbContextBase
    {
        public GoodDbContext_Development_(DbContextOptions options) : base(options) { }
    }
}
