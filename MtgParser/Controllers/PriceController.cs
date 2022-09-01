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


    [HttpPost(Name = "FillPrices")]
    public async Task<ActionResult> FillPrices()
    {
        try
        {
            List<CardSet> source = _dbContext.CardsSets.Include(x => x.Card)
                                                       .Include(x => x.Set)
                                                       .Include(x => x.Prices)
                                                       .ToList();

            foreach (CardSet cardRequest in source)
            {
                try
                {
                    Price price = await _parseService.GetPriceAsync(cardRequest);
                    if (cardRequest.Prices.MaxBy(x => x.CreateDate)?.Value != price.Value)
                    {
                        _dbContext.Prices.Add(price);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"can't find price for card {cardRequest.Card.Name} set {cardRequest.Set.ShortName} ");
                }
            }
            
            await _dbContext.SaveChangesAsync();
            return new OkResult();
        }
        catch (Exception e)
        {
            _logger.LogCritical($"FillPrices error {e.Message + Environment.NewLine + e.StackTrace}");
            return new BadRequestResult();
        }
    }
}