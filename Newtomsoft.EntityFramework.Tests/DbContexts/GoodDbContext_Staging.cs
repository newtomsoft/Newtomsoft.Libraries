using Microsoft.EntityFrameworkCore;

namespace Newtomsoft.EntityFramework.Tests.DbContexts;

public class GoodDbContext_Staging : GoodDbContextBase
{
    public GoodDbContext_Staging(DbContextOptions options) : base(options) { }
}