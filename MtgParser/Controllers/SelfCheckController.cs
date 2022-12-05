using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MtgParser.Context;
using MtgParser.Model;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SelfCheckController : ControllerBase
{
    private readonly MtgContext _dbContext;
    
    public SelfCheckController(MtgContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetUnparsedCards")]
    public async Task<List<CardName>> GetUnparsedCards()
    {
        List<CardName> source = await _dbContext.CardsNames.ToListAsync();
        List<Card> cards = await _dbContext.Cards.ToListAsync();

        return source.Where(x => cards.All(y => !x.IsCardEqual(y))).ToList();
    }
}