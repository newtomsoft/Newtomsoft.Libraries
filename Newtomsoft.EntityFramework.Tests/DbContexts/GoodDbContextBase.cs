using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtomsoft.EntityFramework.Tests.Models;

namespace Newtomsoft.EntityFramework.Tests.DbContexts;

public class GoodDbContextBase : DbContext
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    public DbSet<CountryModel> Countries { get; set; }
    
    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    public DbSet<CityModel> Cities { get; set; }

    public GoodDbContextBase(DbContextOptions options) : base(options) { }
}