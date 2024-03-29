using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MtgParser.Context;
using MtgParser.Model;

namespace MtgParser.Controllers;

/// <summary>
/// check db for parse errors and other issues.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class SelfCheckController : ControllerBase
{
    private readonly MtgContext _dbContext;

    /// <inheritdoc />
    public SelfCheckController(MtgContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// получить спискок карт, которые не получилось распарсить
    /// </summary>
    /// <returns>cardName, которые надо поправить или перепарсить</returns>
    [HttpGet]
    public async Task<List<CardName>> GetUnparsedCardsAsync()
    {
        List<CardName> source = await _dbContext.CardsNames.AsNoTracking().ToListAsync();
        List<Card> cards = await _dbContext.Cards.AsNoTracking().ToListAsync();

        return source.Where(x => cards.All(y => !x.IsCardEqual(y))).ToList();
    }
    
    /// <summary>
    /// получить спискок карт, которые почему-то попали в базу более одного раза..
    /// </summary>
    /// <returns>cardName, которые надо поправить или перепарсить</returns>
    [HttpGet]
    public List<Card> GetDuplicateCards()
    {
        DbSet<Card> cards = _dbContext.Cards;

        return cards.Where(x => cards.Any(y => (x.IsRus == y.IsRus && x.Id != y.Id) &&
                                                        ((x.IsRus && x.NameRus == y.NameRus) 
                                                        || (!x.IsRus && x.Name == y.Name)))).AsNoTracking().ToList();
    }
    
    /// <summary>
    /// отладочное апи. получает текущую версию приложения
    /// </summary>
    /// <returns> major.minor.build.revision</returns>
    [HttpGet]
    public string GetVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}