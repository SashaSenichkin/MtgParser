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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    public ImagesController(ILogger<ImagesController> logger, MtgContext dbContext)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    /// <summary>
    /// Replace All db.Cards
    /// </summary>
    /// <param name="newRoot"></param>
    /// <returns></returns>
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

    /// <summary>
    /// takes all pictures from Cards.img and save new to wwwroot
    /// </summary>
    [HttpGet]
    public void DownloadCardImages()
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
                string? newFileName = await DownloadImageAsync(card.Img);
                if (!string.IsNullOrEmpty(newFileName))
                {
                    _logger.LogInformation("downloaded image {Img} to {path}", card.Img, newFileName);
                }
                else
                {
                    _logger.LogInformation("image {Img} already exists", card.Img);
                }
            }
            catch (Exception e)
            {
                _logger.LogError( "SaveCardImages fail on card {Id} Msg {Message} Stack {Stack}", card.Id, e.Message, e.StackTrace);
            }
        }
    }

    private static async Task<string?> DownloadImageAsync(string url)
    {
        string machineRoot;
        if ( Environment.OSVersion.Platform == PlatformID.Unix)
        {
            machineRoot = "/app/wwwroot/images";
        }
        else
        {
            machineRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\", "wwwroot", "images");
        }
        
        string fileName = GetNewFilePath(url, machineRoot);
        if (Exists(fileName))
        {
            return null;  
        }

        Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(url);
        await using FileStream imageFile = new(fileName, FileMode.Create);
        await response.Content.CopyToAsync(imageFile);
        return fileName;
    }

    private static string GetNewFilePath(string oldPath, string newPart)
    {
        int setIndex = oldPath[..oldPath.LastIndexOf('/')].LastIndexOf('/');
        string setAndName = oldPath[setIndex..];
        
        return newPart + setAndName;
    }
}