using Serilog;
using ImageService.Context;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ImageService API"
    });
    options.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ImageService.xml"));
});

string? connectionString = builder.Configuration.GetConnectionString("MtgContext");
builder.Services.AddSqlServer<MtgContext>(connectionString);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();