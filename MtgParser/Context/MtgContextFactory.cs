using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MtgParser.Context;

/// <summary>
/// migration helper
/// </summary>
public class MtgContextFactory : IDesignTimeDbContextFactory<MtgContext>
{
    public MtgContext CreateDbContext(string[] args)
    {
        // Get environment
        string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // Build config
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Environment.CurrentDirectory))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        DbContextOptionsBuilder<MtgContext> optionsBuilder = new();
        string? connectionString = config.GetConnectionString("MtgContext");
        ServerVersion? version = ServerVersion.AutoDetect(connectionString);
        optionsBuilder.UseMySql(connectionString, version);
        
        return new MtgContext(optionsBuilder.Options);
    }
}