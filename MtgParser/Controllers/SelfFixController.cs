using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MtgParser.Context;
using MtgParser.Model;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SelfFixController : ControllerBase
{
    private readonly MtgContext _dbContext;
    private readonly ILogger<ParseManyController> _logger;

    public SelfFixController(MtgContext dbContext, ILogger<ParseManyController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost(Name = "TrimCardNames")]
    public async Task<bool> TrimCardNamesAsync()
    {
        try
        {
            DbSet<CardName> source = _dbContext.CardsNames;
            foreach (CardName item in source)
            {
                item.Name = item.Name?.Trim();
                item.NameRus = item.NameRus?.Trim();
                item.SetShort = item.SetShort.Trim();
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("SelfFixController. TrimCardNamesAsync {error}", e.Message + e.StackTrace);
            return false;
        }
    }
}