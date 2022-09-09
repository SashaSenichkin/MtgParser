using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtgParser.Context;
using MtgParser.ParseLogic;
using Serilog;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
        
builder.Host.UseSerilog((context, services, configuration) => configuration
       .ReadFrom.Configuration(context.Configuration)
       .ReadFrom.Services(services)
       .Enrich.FromLogContext()
       .WriteTo.Console());
        
builder.Services.AddScoped<ParseCardSet>();

string? connectionString = builder.Configuration.GetConnectionString("MtgContext");
ServerVersion? version = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<MtgContext>(options => options.UseMySql(connectionString, version));
        
WebApplication app = builder.Build();
        
using IServiceScope scope = (app as IApplicationBuilder).ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
scope.ServiceProvider.GetService<MtgContext>()?.Database.Migrate();

app.UseSwagger();
app.UseSwaggerUI();
app.UseSerilogRequestLogging();   
app.UseHttpsRedirection();
app.MapControllers();
app.Run();