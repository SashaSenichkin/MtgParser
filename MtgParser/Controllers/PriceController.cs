using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.Provider;

namespace MtgParser.Controllers;

/// <summary>
/// auto price getter
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class PriceController : ControllerBase
{
    private readonly IPriceProvider _priceProvider;
    private readonly ILogger<PriceController> _logger;
    private readonly MtgContext _dbContext;

    /// <inheritdoc />
    public PriceController(MtgContext dbContext, IPriceProvider priceProvider,  ILogger<PriceController> logger)
    {
        _priceProvider = priceProvider;
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// тестовое получение цены по физическому представлениею карты
    /// </summary>
    /// <param name="cardSetId">выбрать id из базы</param>
    /// <returns>цена данной карты</returns>
    [HttpGet]
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

            Price price = await _priceProvider.GetPriceAsync(cardSet);
            return new OkObjectResult(price);
        }
        catch (Exception e)
        {
            _logger.LogCritical("GetPrice error {Message}", e.Message + Environment.NewLine + e.StackTrace);
            return new BadRequestResult();
        }
    }

    /// <summary>
    /// заберёт всю информацию из базы, постарается найти цены на все CardSet
    /// </summary>
    /// <returns>успех-провал</returns>
    [HttpPost]
    public async Task<ActionResult> FillPricesAsync()
    {
        try
        {
            List<CardSet> source = await _dbContext.CardsSets.Include(x => x.Card)
                                                             .Include(x => x.Set)
                                                             .Include(x => x.Prices)
                                                             .ToListAsync();

            foreach (CardSet cardRequest in source)
            {
                try
                {
                    Price price = await _priceProvider.GetPriceAsync(cardRequest);
                    if (cardRequest.Prices.MaxBy(x => x.CreateDate)?.Value != price.Value)
                    {
                        _dbContext.Prices.Add(price);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "can't find price for card {Card} set {Set} ", 
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