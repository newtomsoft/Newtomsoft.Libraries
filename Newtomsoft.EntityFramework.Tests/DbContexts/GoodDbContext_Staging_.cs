using Microsoft.EntityFrameworkCore;
using Newtomsoft.EntityFramework.Tests.Models;

namespace Newtomsoft.EntityFramework.Tests.DbContexts
{
    public class GoodDbContext_Staging_ : GoodDbContextBase
    {
        public GoodDbContext_Staging_(DbContextOptions options) : base(options) { }
    }
}
