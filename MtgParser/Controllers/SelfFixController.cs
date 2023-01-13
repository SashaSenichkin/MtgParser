using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MtgParser.Context;
using MtgParser.Model;

namespace MtgParser.Controllers;

/// <summary>
/// fix simple staff.. dont add here methods, if you not sure that it will not broke something in future.. better save one-time sql
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class SelfFixController : ControllerBase
{
    private readonly MtgContext _dbContext;
    private readonly ILogger<ParseManyController> _logger;

    /// <inheritdoc />
    public SelfFixController(MtgContext dbContext, ILogger<ParseManyController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    /// <summary>
    /// обрезает лишнее от внесённых вручную карт
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> TrimCardNamesAsync()
    {
        try
        {
            DbSet<CardName> source = _dbContext.CardsNames;
            foreach (CardName item in source)
            {
                item.Name = item.Name?.Trim();
                item.NameRus = item.NameRus?.Trim();
                item.SetShort = item.SetShort.Trim();
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("SelfFixController. TrimCardNamesAsync {error}", e.Message + e.StackTrace);
            return false;
        }
    }
}