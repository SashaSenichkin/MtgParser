using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MtgParser.Context;
using MtgParser.Model;
using MtgParser.ParseLogic;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]")]
public class ParseController : ControllerBase
{
    private readonly ParseService _parseService;
    private readonly MtgContext _dbContext;


    public ParseController(MtgContext dbContext, ParseService parseService)
    {
        _parseService = parseService;
        _dbContext = dbContext;
    }
    
    [HttpGet(Name = "GetCardInfo")]
    public async Task<Card> Get(string cardName)
    {
        return await _parseService.ParseOneCard(cardName);
    }
}