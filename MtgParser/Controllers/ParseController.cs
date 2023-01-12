using Microsoft.AspNetCore.Mvc;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.Provider;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ParseController : ControllerBase
{
    private readonly ICardSetProvider _cardSetProvider;
    private readonly MtgContext _dbContext;
    private readonly ILogger<ParseController> _logger;
    
    public ParseController(MtgContext dbContext, ILogger<ParseController> logger, ICardSetProvider cardSetProvider)
    {
        _cardSetProvider = cardSetProvider;
        _dbContext = dbContext;
        _logger = logger;
    }
    
    /// <summary>
    /// проверка общего механизма получения информации по названию
    /// </summary>
    /// <param name="cardName">название карты</param>
    /// <returns>вся информация, которую мы смогли получить</returns>
    [HttpGet]
    public async Task<Card> GetCardInfoAsync(string cardName)
    {
        CardSet cardSet = await _cardSetProvider.GetCardSetAsync(new CardName() { Name = cardName });
        return cardSet.Card;
    }
    
    /// <summary>
    /// проверка получение физического представления карты, с максимальной информацией, которую можем получить из других источников
    /// </summary>
    /// <param name="cardName">имя карты</param>
    /// <param name="setShortName">аббревиатура сета. сет создастся в базе, если не найдётся в существующих</param>
    /// <returns>вся информация, которую мы смогли получить</returns>
    [HttpGet]
    public async Task<CardSet> GetCardSetInfoAsync(string cardName, string setShortName)
    {
        return await _cardSetProvider.GetCardSetAsync(new CardName() { Name = cardName, SetShort = setShortName} );
    }
    
    /// <summary>
    /// сохранение в базу данных информации по одной карте
    /// </summary>
    /// <param name="cardName">имя карты</param>
    /// <returns>успешность операции</returns>
    [HttpPost]
    public async Task<bool> PostToDbAsync(string cardName)
    {
        try
        {
            CardSet cardSet = await _cardSetProvider.GetCardSetAsync(new CardName() { Name = cardName });
            _logger.LogInformation("PostToDb found cardSet {cardId} {cardName}", cardSet.Id, cardSet.Card.Name);

            Card card = cardSet.Card;
            await _dbContext.Cards.AddAsync(card);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PostToDb fails");
            return false;
        }
    }
}