using Microsoft.EntityFrameworkCore;
using Newtomsoft.EntityFramework.Tests.Models;

namespace Newtomsoft.EntityFramework.Tests.DbContexts;

public class BadProviderDbContext : DbContext
{
    public DbSet<CountryModel> Countries { get; set; }
    public DbSet<CityModel> Cities { get; set; }

    public BadProviderDbContext(DbContextOptions<BadProviderDbContext> options) : base(options)
    {
        // Method intentionally left empty.
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Method intentionally left empty.
    }
}