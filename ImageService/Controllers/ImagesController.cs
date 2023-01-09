using System.Reflection;
using ImageService.Context;
using Microsoft.AspNetCore.Mvc;
using static System.IO.File;

namespace ImageService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ImagesController : ControllerBase
{
    private readonly MtgContext _dbContext;
    private readonly ILogger<ImagesController> _logger;
    private readonly IConfiguration _fullConfig;

    private const string MtgAddressPart = "http://www.mtg.ru/pictures";

    public ImagesController(ILogger<ImagesController> logger, MtgContext dbContext, IConfiguration fullConfig)
    {
         _dbContext = dbContext;
        _logger = logger;
        _fullConfig = fullConfig;
    }
    [HttpPost]
    public async Task<bool> SetCardImagePaths(string newRoot)
    {
        try
        {
            List<Card> allCards = _dbContext.Cards.ToList();
            foreach (Card card in allCards)
            {
                card.Img = GetNewFilePath(card.Img, newRoot);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError( "SetCardImagePaths fail Msg {Message} Stack {Stack}",  e.Message, e.StackTrace);
            return false;
        }
    }

    [HttpGet]
    public void SetCardImages()
    {
        List<Card> allCards = _dbContext.Cards.ToList();
        Thread worker = new(() => SaveCardImages(allCards));
        worker.Start();
    }

    private async void SaveCardImages(List<Card> allCards)
    {
        foreach (Card card in allCards)
        {
            try
            {
                await DownloadImageAsync(card.Img);
            }
            catch (Exception e)
            {
                _logger.LogError( "SaveCardImages fail on card {Id} Msg {Message} Stack {Stack}", card.Id, e.Message, e.StackTrace);
            }
        }
    }

    private static async Task DownloadImageAsync(string url)
    {
        string fileName = GetNewFilePath(url, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\", "wwwroot", "images"));
        if (Exists(fileName))
        {
            return;  
        }

        Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(url);
        await using FileStream imageFile = new(fileName, FileMode.Create);
        await response.Content.CopyToAsync(imageFile);
    }

    private static string GetNewFilePath(string oldPath, string newPart)
    {
        int setIndex = oldPath[..oldPath.LastIndexOf('/')].LastIndexOf('/');
        string setAndName = oldPath[setIndex..];
        
        return newPart + setAndName;
    }
}