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
    
    /// <summary>
    /// проверка общего механизма получения информации по названию
    /// </summary>
    /// <param name="cardName">название карты</param>
    /// <returns>вся информация, которую мы смогли получить</returns>
    [HttpGet(Name = "GetCardInfo")]
    public async Task<Card> GetCardInfo(string cardName)
    {
        return await _parseService.GetCardAsync(cardName);
    }
    
    /// <summary>
    /// проверка получение физического представления карты, с максимальной информацией, которую можем получить из других источников
    /// </summary>
    /// <param name="cardName">имя карты</param>
    /// <param name="setShortName">аббревиатура сета. сет создастся в базе, если не найдётся в существующих</param>
    /// <returns>вся информация, которую мы смогли получить</returns>
    [HttpGet(Name = "GetCardSetInfo")]
    public async Task<CardSet> GetCardSetInfo(string cardName, string setShortName)
    {
        return await _parseService.GetCardSetAsync(new CardName() { Name = cardName, SetShort = setShortName} );
    }
    
    /// <summary>
    /// сохранение в базу данных информации по одной карте
    /// </summary>
    /// <param name="cardName">имя карты</param>
    /// <returns>успешность операции</returns>
    [HttpPost(Name = "PostToDb")]
    public async Task<bool> PostToDb(string cardName)
    {
        try
        {
            Card card = await _parseService.GetCardAsync(cardName);
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