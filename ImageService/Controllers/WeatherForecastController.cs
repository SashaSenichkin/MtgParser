using ImageService.Context;
using Microsoft.AspNetCore.Mvc;

namespace ImageService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private readonly MtgContext _dbContext;
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IConfiguration _fullConfig;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, MtgContext dbContext, IConfiguration fullConfig)
    {
         _dbContext = dbContext;
        _logger = logger;
        _fullConfig = fullConfig;
    }

    [HttpGet(Name = "test")]
    public void Get()
    {
        
        List<Card> Cards = _dbContext.Cards.ToList();
        Console.WriteLine(Cards.Count);

        Console.WriteLine(_fullConfig.GetConnectionString("MtgContext") + 1);
    }
}