using System.Reflection;
using ImageService.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.IO.File;

namespace ImageService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SelfCheckController : ControllerBase
{
    private readonly MtgContext _dbContext;
    private readonly ILogger<ImagesController> _logger;


    public SelfCheckController(ILogger<ImagesController> logger, MtgContext dbContext)
    {
         _dbContext = dbContext;
        _logger = logger;
    }


    [HttpGet]
    public string[] CheckCardImages()
    {
        List<Card> allCards = _dbContext.Cards.ToList();
        string[] result = allCards.Where(x => !Exists(x.Img)).Select(x => x.Img).ToArray();
        return result;
    }
    
    [HttpGet]
    public async Task<string[]> CheckRusImages()
    {
        List<Card> allCards = await _dbContext.Cards.ToListAsync();
        IEnumerable<string> result = allCards.Where(x => x.IsRus && !x.Img.Contains("_RUS/")).Select(x => x.Img)
                              .Union(allCards.Where(x => !x.IsRus && x.Img.Contains("_RUS/")).Select(x => x.Img));
        
        return result.ToArray();
    }
}