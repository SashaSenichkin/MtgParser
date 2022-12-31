using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MtgParser.Context;
using MtgParser.Model;

namespace MtgParser.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SelfFixController : ControllerBase
{
    private readonly MtgContext _dbContext;
    
    public SelfFixController(MtgContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost(Name = "TrimCardNames")]
    public async Task<bool> TrimCardNames()
    {
        DbSet<CardName> source = _dbContext.CardsNames;
        foreach (CardName item in source)
        {
            item.Name = item.Name.Trim();
            item.NameRus = item.NameRus.Trim();
            item.SetShort = item.SetShort.Trim();
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}