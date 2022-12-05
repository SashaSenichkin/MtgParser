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
    private readonly PriceParser _priceParser;
    private readonly ILogger<PriceController> _logger;
    private readonly MtgContext _dbContext;
    
    public PriceController(MtgContext dbContext, PriceParser priceParser,  ILogger<PriceController> logger)
    {
        _priceParser = priceParser;
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
                _logger.LogError("can't find cardSet by id {SetId}", cardSetId);
                return new BadRequestResult();
            }

            Price price = await _priceParser.GetPriceAsync(cardSet, default);
            return new OkObjectResult(price);
        }
        catch (Exception e)
        {
            _logger.LogCritical("GetPrice error {Message}", e.Message + Environment.NewLine + e.StackTrace);
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
                    Price price = await _priceParser.GetPriceAsync(cardRequest, default);
                    if (cardRequest.Prices.MaxBy(x => x.CreateDate)?.Value != price.Value)
                    {
                        _dbContext.Prices.Add(price);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("can't find price for card {Card} set {Set} ", 
                                    cardRequest.Card.Name, 
                                    cardRequest.Set.ShortName);
                }
            }
            
            await _dbContext.SaveChangesAsync();
            return new OkResult();
        }
        catch (Exception e)
        {
            _logger.LogCritical("FillPrices error {Message}", e.Message + Environment.NewLine + e.StackTrace);
            return new BadRequestResult();
        }
    }
}