using Microsoft.EntityFrameworkCore;
using Newtomsoft.EntityFramework.Tools.Demo.Repository.Models;

namespace Newtomsoft.EntityFramework.Tools.Demo.Repository.DbContexts
{
    public class DefaultDbContext : DbContext
    {
        public DbSet<CountryModel> Countries { get; set; }
        public DbSet<CityModel> Cities { get; set; }


        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
        {
            // Method intentionally left empty.
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Method intentionally left empty.
        }
    }
}
