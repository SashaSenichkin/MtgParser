using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MtgParser.Model;

namespace MtgParser.Context;

public class MtgContext : DbContext
{
    public MtgContext(DbContextOptions options):base(options)
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
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // Build config
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Environment.CurrentDirectory))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        DbContextOptionsBuilder<MtgContext> optionsBuilder = new DbContextOptionsBuilder<MtgContext>();
        string? connectionString = config.GetConnectionString("MtgContext");
        ServerVersion? version = ServerVersion.AutoDetect(connectionString);
        optionsBuilder.UseMySql(connectionString, version);
        
        return new MtgContext(optionsBuilder.Options);
    }
}