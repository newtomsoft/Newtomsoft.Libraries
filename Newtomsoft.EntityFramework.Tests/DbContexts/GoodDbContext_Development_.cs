using Microsoft.EntityFrameworkCore;

namespace Newtomsoft.EntityFramework.Tests.DbContexts
{
    public class GoodDbContext_Development_ : GoodDbContextBase
    {
        public GoodDbContext_Development_(DbContextOptions options) : base(options)
        {
            // Method intentionally left empty.
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // Method intentionally left empty.
        //}
    }
}
