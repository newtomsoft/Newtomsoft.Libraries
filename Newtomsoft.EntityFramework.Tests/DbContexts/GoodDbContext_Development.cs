using Microsoft.EntityFrameworkCore;

namespace Newtomsoft.EntityFramework.Tests.DbContexts;

public class GoodDbContext_Development : GoodDbContextBase
{
    public GoodDbContext_Development(DbContextOptions options) : base(options) { }
}