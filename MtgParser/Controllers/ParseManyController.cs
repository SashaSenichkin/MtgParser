using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.ParseLogic;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]")]
public class ParseManyController : ControllerBase
{
    private readonly ParseCardSet _parseCardSet;
    private readonly ILogger<ParseManyController> _logger;
    private readonly MtgContext _dbContext;
    
    public ParseManyController(MtgContext dbContext, ParseCardSet parseCardSet, ILogger<ParseManyController> logger)
    {
        _parseCardSet = parseCardSet;
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// проходится по всем записям в таблице CardNames, пытается получить информацию с сайтов и сохранить в нашем виде
    /// </summary>
    /// <returns>Общая успешность обработки. смотри лог, в случае глобальных ошибок и для частных, которые не влияют на общую успешность</returns>
    [HttpPost(Name = "PostMany")]
    public async Task<bool> PostToDb()
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
            CardSet cardSet = await _parseCardSet.GetCardSetAsync(cardRequest);
            if (cardSet.Id == default)
            {
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