using Microsoft.EntityFrameworkCore;
using Newtomsoft.EntityFramework.Tests.Models;

namespace Newtomsoft.EntityFramework.Tests.DbContexts;

public class GoodDbContextBase : DbContext
{
    public DbSet<CountryModel> Countries { get; set; }
    public DbSet<CityModel> Cities { get; set; }

    public GoodDbContextBase(DbContextOptions options) : base(options) { }
}