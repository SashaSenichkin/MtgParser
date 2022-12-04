using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.ParseLogic;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ParseManyController : ControllerBase
{
    private readonly CardSetParser _cardSetParser;
    private readonly ILogger<ParseManyController> _logger;
    private readonly MtgContext _dbContext;
    
    public ParseManyController(MtgContext dbContext, CardSetParser cardSetParser, ILogger<ParseManyController> logger)
    {
        _cardSetParser = cardSetParser;
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// загрузить имена для дальнейшей разборки
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost(Name = "PostCardNamesInfo")]
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
    /// удалить все данные, кроме справочников Rarity, Keywords и вручную заполняемой таблицы CardNames
    /// </summary>
    /// <returns></returns>
    [HttpDelete(Name = "ClearParsedData")]
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
    [HttpPost(Name = "ParceAllCardNamesToDb")]
    public async Task<bool> ParceAllCardNamesToDb()
    {
        try
        {
            List<CardName> source = _dbContext.CardsNames.ToList();
            foreach (CardName cardRequest in source)
            {
                await ProcessOneCardName(cardRequest);
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

    private async Task ProcessOneCardName(CardName cardRequest)
    {
        try
        {
            CardSet cardSet = await _cardSetParser.GetCardSetAsync(cardRequest);
            if (cardSet.Id == default)
            {
                _logger.LogInformation($"add card {cardSet.Card.Name} {cardSet.Card.NameRus} {cardSet.Rarity.Name} + {cardSet.Set.ShortName}");
                await _dbContext.CardsSets.AddAsync(cardSet);
            }
        }
        catch (Exception e)
        {
            _logger.LogError("error on card {Name}  {SetShort} error {Message}",
                            cardRequest.Name ?? cardRequest.NameRus,
                            cardRequest.SetShort,
                            e.Message + Environment.NewLine + e.StackTrace);
        }
    }
}