using Microsoft.EntityFrameworkCore;
using Newtomsoft.EntityFramework.Tools.Demo.Repository.Models;

namespace Newtomsoft.EntityFramework.Tools.Demo.Repository.DbContexts
{
    public class OtherDbContext : DbContext
    {
        public DbSet<PlanetModel> Planets { get; set; }
        public DbSet<ContinentModel> Continents { get; set; }


        public OtherDbContext(DbContextOptions<OtherDbContext> options) : base(options)
        {
            // Method intentionally left empty.
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Method intentionally left empty.
        }
    }
}
