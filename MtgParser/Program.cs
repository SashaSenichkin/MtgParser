using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using MtgParser.Context;
using MtgParser.ParseLogic;
using MtgParser.Provider;
using Serilog;

DotNetEnv.Env.TraversePath().Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
       options.SwaggerDoc("v1", new OpenApiInfo
       {
              Version = "v1",
              Title = "MtgParser API"
       });
       options.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "MtgParser.xml"));
});
        
builder.Host.UseSerilog((context, services, configuration) => configuration
       .ReadFrom.Configuration(context.Configuration)
       .ReadFrom.Services(services)
       .Enrich.FromLogContext()
       .WriteTo.Console());
        
builder.Services.AddScoped<PriceParser>();
builder.Services.AddScoped<CardSetParser>();

builder.Services.AddScoped<IPriceProvider, PriceProvider>();
builder.Services.AddScoped<ICardSetProvider, CardSetProvider>();


string? connectionString = builder.Configuration.GetConnectionString("MtgContext");

builder.Services.AddMySql<MtgContext>(connectionString, ServerVersion.AutoDetect(connectionString));
        
WebApplication app = builder.Build();
        
using IServiceScope scope = (app as IApplicationBuilder).ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
scope.ServiceProvider.GetService<MtgContext>()?.Database.Migrate();

app.UseSwagger();
app.UseSwaggerUI();
app.UseSerilogRequestLogging();   
app.UseHttpsRedirection();
app.MapControllers();
app.Run();