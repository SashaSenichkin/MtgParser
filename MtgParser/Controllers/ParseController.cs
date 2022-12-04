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
    private readonly CardSetParser _cardSetParser;
    private readonly MtgContext _dbContext;
    
    public ParseController(MtgContext dbContext, CardSetParser cardSetParser)
    {
        _cardSetParser = cardSetParser;
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
        return await _cardSetParser.GetCardAsync(cardName);
    }
    
    /// <summary>
    /// пинг бэка для проверки работоспособности
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetTestInfo")]
    public String GetTestInfo()
    {
        return "All systems on-line";
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
        return await _cardSetParser.GetCardSetAsync(new CardName() { Name = cardName, SetShort = setShortName} );
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
            Card card = await _cardSetParser.GetCardAsync(cardName);
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