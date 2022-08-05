using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.ParseLogic;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]")]
public class ParseManyController : ControllerBase
{
    private readonly ParseService _parseService;
    private readonly MtgContext _dbContext;


    public ParseManyController(MtgContext dbContext, ParseService parseService)
    {
        _parseService = parseService;
        _dbContext = dbContext;
    }
    
    
    [HttpPost(Name = "PostMany")]
    public async Task<bool> PostToDb()
    {
        try
        {
            List<CardName> source = _dbContext.CardsNames.ToList();
            foreach (CardName cardRequest in source)
            {
                CardSet cardSet = await _parseService.ParseOneCardSet(cardRequest);
                if (cardSet.Id != default)
                {
                    await _dbContext.CardsSets.AddAsync(cardSet);
                }
            }
            
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