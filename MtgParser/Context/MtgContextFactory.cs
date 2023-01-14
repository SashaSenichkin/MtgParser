using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MtgParser.Context;

/// <summary>
/// migration helper
/// </summary>
public class MtgContextFactory : IDesignTimeDbContextFactory<MtgContext>
{
    /// <inheritdoc />
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
        optionsBuilder.UseSqlServer(connectionString);
        
        return new MtgContext(optionsBuilder.Options);
    }
}