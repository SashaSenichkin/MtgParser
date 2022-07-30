using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MtgParser.Context;

namespace MtgParser;

public class Program
{
    public static void Main(string[] args)
    {
        CheckAndUpMigrations();
        var builder = WebApplication.CreateBuilder(args);
        

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var version = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MtgConnection"));
        builder.Services.AddDbContext<MtgContext>(options => options.UseMySql(version));
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }

    private static void CheckAndUpMigrations()
    {
        throw new NotImplementedException();
    }
}