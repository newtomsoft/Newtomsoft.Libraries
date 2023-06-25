using Microsoft.EntityFrameworkCore;

namespace Newtomsoft.EntityFramework.Tests.DbContexts;

public class NoConnectionStringForThisDbContext : DbContext
{
    public NoConnectionStringForThisDbContext(DbContextOptions<NoConnectionStringForThisDbContext> options) : base(options)
    {
        // Method intentionally left empty.
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Method intentionally left empty.
    }
}