using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.Provider;
using Newtonsoft.Json;

namespace MtgParser.Controllers;

/// <summary>
/// Api methods for automation mass parse, save to db and clear all.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class ParseManyController : ControllerBase
{
    private readonly ICardSetProvider _cardSetProvider;
    private readonly ILogger<ParseManyController> _logger;
    private readonly MtgContext _dbContext;

    /// <inheritdoc />
    public ParseManyController(MtgContext dbContext, ICardSetProvider cardSetProvider, ILogger<ParseManyController> logger)
    {
        _cardSetProvider = cardSetProvider;
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// загрузить имена для дальнейшей разборки
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost]
    public bool PostCardNamesInfo(IEnumerable<CardName> data)
    {
        try
        {
            foreach (CardName card in data)
            {
                _dbContext.CardsNames.AddAsync(card);
            }
            _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    /// <summary>
    /// получить информацию по именам для теста
    /// </summary>
    /// <param name="dataRaw"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IEnumerable<CardSet>> GetCardNamesInfoAsync(string dataRaw)
    {
        try
        {
            CardName[]? data = JsonConvert.DeserializeObject<CardName[]>(dataRaw);
            List<CardSet> result = new();
            foreach (CardName card in data)
            {
                result.Add(await _cardSetProvider.GetCardSetAsync(card));
            }

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    /// <summary>
    /// удалить все данные, кроме справочников Rarity, Keywords и вручную заполняемой таблицы CardNames
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    public bool ClearParsedData()
    {
        try
        {
            _dbContext.CardsSets.RemoveRange(_dbContext.CardsSets);
            _dbContext.Prices.RemoveRange(_dbContext.Prices);
            _dbContext.Cards.RemoveRange(_dbContext.Cards);
            _dbContext.Sets.RemoveRange(_dbContext.Sets);
            
            _dbContext.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    /// <summary>
    /// проходится по всем записям в таблице CardNames, пытается получить информацию с сайтов и сохранить в нашем виде
    /// </summary>
    /// <returns>Общая успешность обработки. смотри лог, в случае глобальных ошибок и для частных, которые не влияют на общую успешность</returns>
    [HttpPost]
    public async Task<bool> ParseAllCardNamesToDbAsync()
    {
        try
        {
            List<CardName> source = await _dbContext.CardsNames.AsNoTracking().ToListAsync();
            foreach (CardName cardRequest in source)
            {
                await ProcessOneCardNameAsync(cardRequest);
            }
            
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogCritical("some general error {Message}",
                e.Message + Environment.NewLine + e.StackTrace);
            
            return false;
        }
    }

    private async Task ProcessOneCardNameAsync(CardName cardRequest)
    {
        if (string.IsNullOrEmpty(cardRequest.SeekName))
        {
            _logger.LogWarning("empty request on id {Id}", cardRequest.Id);
            return;
        }
        
        try
        {
            CardSet cardSet = await _cardSetProvider.GetCardSetAsync(cardRequest);
            if (cardSet.Id == default)
            {
                _logger.LogInformation("add card {CardName} {CardNameRus} {Rarity} + {SetShortName}", cardSet.Card.Name, cardSet.Card.NameRus, cardSet.Rarity.Name, cardSet.Set.ShortName);
                await _dbContext.CardsSets.AddAsync(cardSet);
            }
        }
        catch (Exception e)
        {
            _logger.LogError("error on card {Name} {SetShort} error {Message}",
                            cardRequest.SeekName,
                            cardRequest.SetShort,
                            e.Message + Environment.NewLine + e.StackTrace);
        }
    }
}