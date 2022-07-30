using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MtgParser.Context;
using MtgParser.Model;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]")]
public class ParceController : ControllerBase
{
    private readonly MtgContext _dbcontext;

    public ParceController(MtgContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    
    [HttpGet(Name = "GetCardInfo")]
    public Card Get()
    {
        var parser = new ParseLogic();
        return parser.
    }
}