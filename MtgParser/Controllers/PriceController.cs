using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.ParseLogic;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]")]
public class PriceController : ControllerBase
{
    private readonly ParseService _parseService;
    private readonly ILogger _logger;
    private readonly MtgContext _dbContext;
    
    public PriceController(MtgContext dbContext, ParseService parseService, ILogger<ParseManyController> logger)
    {
        _parseService = parseService;
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// тестовое получение цены по физическому представлениею карты
    /// </summary>
    /// <param name="cardSetId">выбрать id из базы</param>
    /// <returns>цена данной карты</returns>
    [HttpGet(Name = "GetPrice")]
    public async Task<ActionResult> GetPrice(int cardSetId)
    {
        try
        {
            CardSet? cardSet = _dbContext.CardsSets.Where(x => x.Id == cardSetId)
                                                   .Include(x => x.Card)
                                                   .Include(x => x.Set)
                                                   .Include(x => x.Prices)
                                                   .FirstOrDefault();

            if (cardSet == null)
            {
                _logger.LogError($"can't find cardSet by id {cardSetId}");
                return new BadRequestResult();
            }

            Price price = await _parseService.GetPriceAsync(cardSet);
            return new OkObjectResult(price);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"GetPrice error {e.Message + Environment.NewLine + e.StackTrace}");
            return new BadRequestResult();
        }
    }

    private async Task ProcessOneCardName(CardName cardRequest)
    {
        try
        {
            CardSet cardSet = await _parseService.GetCardSetAsync(cardRequest);
            if (cardSet.Id == default)
            {
                await _dbContext.CardsSets.AddAsync(cardSet);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"error on card {cardRequest.Name}  {cardRequest.SetShort} error {e.Message + Environment.NewLine + e.StackTrace}");
        }
    }
}