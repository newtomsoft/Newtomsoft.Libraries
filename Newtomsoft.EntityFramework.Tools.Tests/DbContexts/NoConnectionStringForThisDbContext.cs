using Microsoft.EntityFrameworkCore;
using Newtomsoft.EntityFramework.Tools.Tests.Models;

namespace Newtomsoft.EntityFramework.Tools.Tests.DbContexts
{
    public class NoConnectionStringForThisDbContext : DbContext
    {
        public DbSet<CountryModel> Countries { get; set; }
        public DbSet<CityModel> Cities { get; set; }

        public NoConnectionStringForThisDbContext(DbContextOptions<NoConnectionStringForThisDbContext> options) : base(options)
        {
            // Method intentionally left empty.
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Method intentionally left empty.
        }
    }
}
