using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MtgParser.Context;
using MtgParser.ParseLogic;

namespace MtgParser;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
 
        builder.Services.AddScoped<ParseService>();

        string? connectionString = builder.Configuration.GetConnectionString("MtgContext");
        ServerVersion? version = ServerVersion.AutoDetect(connectionString);
        builder.Services.AddDbContext<MtgContext>(options => options.UseMySql(connectionString, version));
        
        WebApplication app = builder.Build();
        
        CheckAndUpdateDb(app);

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.MapControllers();
        app.Run();
    }

    public static void CheckAndUpdateDb(IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        scope.ServiceProvider.GetService<MtgContext>()?.Database.Migrate();
    }
}