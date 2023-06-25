﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtomsoft.EntityFramework.Tests.Models;

namespace Newtomsoft.EntityFramework.Tests.DbContexts;

public class OracleDbContext : DbContext
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    public DbSet<CountryModel> Countries { get; set; }
    
    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    public DbSet<CityModel> Cities { get; set; }

    public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options)
    {
        // Method intentionally left empty.
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Method intentionally left empty.
    }
}