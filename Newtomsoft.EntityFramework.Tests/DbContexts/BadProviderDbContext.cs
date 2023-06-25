using Microsoft.EntityFrameworkCore;

namespace Newtomsoft.EntityFramework.Tests.DbContexts;

public class BadProviderDbContext : DbContext
{
    public BadProviderDbContext(DbContextOptions<BadProviderDbContext> options) : base(options)
    {
        // Method intentionally left empty.
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Method intentionally left empty.
    }
}