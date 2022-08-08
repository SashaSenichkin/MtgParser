using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.ParseLogic;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ParseController : ControllerBase
{
    private readonly ParseService _parseService;
    private readonly MtgContext _dbContext;
    
    public ParseController(MtgContext dbContext, ParseService parseService)
    {
        _parseService = parseService;
        _dbContext = dbContext;
    }
    
    [HttpGet(Name = "GetCardInfo")]
    public async Task<Card> GetCardInfo(string cardName)
    {
        return await _parseService.ParseOneCard(cardName);
    }
    
    [HttpGet(Name = "GetCardSetInfo")]
    public async Task<CardSet> GetCardSetInfo(string cardName, string setShortName)
    {
        return await _parseService.ParseOneCardSet(cardName, setShortName);
    }
    
    [HttpPost(Name = "PostToDb")]
    public async Task<bool> PostToDb(string cardName)
    {
        try
        {
            Card card = await _parseService.ParseOneCard(cardName);
            await _dbContext.Cards.AddAsync(card);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}