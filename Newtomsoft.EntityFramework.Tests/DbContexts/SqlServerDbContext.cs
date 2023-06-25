using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtomsoft.EntityFramework.Tests.Models;

namespace Newtomsoft.EntityFramework.Tests.DbContexts;

public class SqlServerDbContext : DbContext
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    public DbSet<CountryModel> Countries { get; private set; }

    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    public DbSet<CityModel> Cities { get; private set; }

    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options)
    {
        // Method intentionally left empty.
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Method intentionally left empty.
    }
}