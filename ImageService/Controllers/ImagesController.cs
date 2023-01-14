using ImageService.Context;
using Microsoft.AspNetCore.Mvc;
using static System.IO.File;

namespace ImageService.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class ImagesController : ControllerBase
{
    private readonly MtgContext _dbContext;
    private readonly ILogger<ImagesController> _logger;

    /// <inheritdoc />
    public ImagesController(ILogger<ImagesController> logger, MtgContext dbContext)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    /// <summary>
    /// Replace All db.Cards and db.Sets
    /// </summary>
    /// <param name="newRoot"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> SetImagesPaths(string newRoot)
    {
        try
        {
            List<Card> allCards = _dbContext.Cards.ToList();
            foreach (Card card in allCards)
            {
                card.Img = GetNewFilePath(card.Img, newRoot);
            }
            
            List<Set> allSets = _dbContext.Sets.ToList();
            foreach (Set set in allSets)
            {
                set.SetImg = GetNewFilePath(set.SetImg, newRoot);
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
        Thread worker = new(() => SaveImages(allCards.Select(x => x.Img)));
        worker.Start();
    }
    
    /// <summary>
    /// takes all pictures from Cards.img and save new to wwwroot
    /// </summary>
    [HttpGet]
    public void DownloadSetImages()
    {
        List<Set> allCards = _dbContext.Sets.ToList();
        Thread worker = new(() => SaveImages(allCards.Select(x => x.SetImg)));
        worker.Start();
    }
    
    /// <summary>
    /// use, if all db urls are correct, but you lost imgages
    /// </summary>
    /// <param name="url">image source root</param>
    [HttpGet]
    public void DownloadAllImagesFromSite(string url)
    {
        List<Card> allCards = _dbContext.Cards.ToList();
        List<Set> allSets = _dbContext.Sets.ToList();
        Thread worker = new(() =>
        {
            SaveImages(allCards.Select(x => GetNewFilePath(x.Img, url)));
            SaveImages(allSets.Select(x => GetNewFilePath(x.SetImg, url)));
        });
        worker.Start();
    }

    private async void SaveImages(IEnumerable<string> allImgs)
    {
        foreach (string img in allImgs)
        {
            try
            {
                string? newFileName = await DownloadImageAsync(img);
                if (!string.IsNullOrEmpty(newFileName))
                {
                    _logger.LogInformation("downloaded image {Img} to {path}", img, newFileName);
                }
                else
                {
                    _logger.LogInformation("image {Img} already exists", img);
                }
            }
            catch (Exception e)
            {
                _logger.LogError( "SaveCardImages fail on img {Img} Msg {Message} Stack {Stack}", img, e.Message, e.StackTrace);
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