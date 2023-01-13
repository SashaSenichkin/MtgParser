using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.Provider;

namespace MtgParser.Controllers;

/// <summary>
/// Parse single cardsSets and safe to DB
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class ParseController : ControllerBase
{
    private readonly ICardSetProvider _cardSetProvider;
    private readonly MtgContext _dbContext;
    private readonly ILogger<ParseController> _logger;

    /// <inheritdoc />
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

    /// <summary>
    /// принудительное обновление информации по конкректным картам. поиск пойдёт по NameRus, если карта русская и по Name, если английская. 
    /// </summary>
    /// <param name="cardIds">оставить пустым для принудительного обновления ВСЕХ КАРТ</param>
    /// <returns>успешность операции</returns>
    [HttpPut]
    public async Task<bool> ReparseCards(int[] cardIds)
    {
        try
        {
            List<Card> cardsToReparse;
            if (cardIds.Any())
            {
                cardsToReparse = await _dbContext.Cards.Where(x => cardIds.Contains(x.Id)).ToListAsync();
            }
            else
            {
                cardsToReparse = await _dbContext.Cards.ToListAsync();
            }
            
            foreach (Card card in cardsToReparse)
            {
                CardName cardName = new();
                if (card.IsRus)
                {
                    cardName.NameRus = card.NameRus;
                }
                else
                {
                    cardName.Name = card.Name;
                }

                CardSet cardSet = await _cardSetProvider.GetDataFromWebAsync(cardName, null, null);
                card.Color = cardSet.Card.Color;
                card.Cmc = cardSet.Card.Cmc;
                card.Text = cardSet.Card.Text;
                card.Img = cardSet.Card.Img;
                card.IsRus = cardSet.Card.IsRus;
                card.Keywords = cardSet.Card.Keywords;
                card.Name = cardSet.Card.Name;
                card.NameRus = cardSet.Card.NameRus;
                card.Power = cardSet.Card.Power;
                card.Toughness = cardSet.Card.Toughness;
                card.Type = cardSet.Card.Type;
                card.TypeRus = cardSet.Card.TypeRus;

                _logger.LogInformation("PostToDb found cardSet {cardId} {cardName}", cardSet.Id, cardSet.Card.Name);

            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ReParseCards fails");
            return false;
        }
    }
}