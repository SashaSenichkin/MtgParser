using System.Security.Policy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MtgParser.Model;

namespace MtgParser.Context;

public class MtgContext : DbContext
{
    public MtgContext(DbContextOptions<MtgContext> options):base(options)
    {}
    
    public DbSet<Keyword> Keywords { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Rarity> Rarities { get; set; }
    public DbSet<Set> Sets { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<CardSet> CardsSets { get; set; }
    
    public DbSet<CardName> CardsNames { get; set; }
    
}

public class MtgContextFactory : IDesignTimeDbContextFactory<MtgContext>
{
    public MtgContext CreateDbContext(string[] args)
    {
        // Get environment
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // Build config
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Environment.CurrentDirectory))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<MtgContext>();
        optionsBuilder.UseMySQL(config.GetConnectionString("MtgConnection"));
        
        return new MtgContext(optionsBuilder.Options);
    }
}