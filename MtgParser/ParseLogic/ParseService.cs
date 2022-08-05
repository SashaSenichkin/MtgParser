using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using Microsoft.Extensions.Configuration;
using MtgParser.Context;
using MtgParser.Model;

using IAngleConfig = AngleSharp.IConfiguration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;


namespace MtgParser.ParseLogic;

public class ParseService
{
    private readonly IConfigurationSection _urlsConfig;
    private readonly MtgContext _context;

    private const string CellSelectorMain = ".SearchCardInfoText";
    private const string CellSelectorInfo = ".SearchCardInfoDIV";
    private const string MtgRuInConfig = "BaseMtgRu";
    private const string MtgRuInfoTableConfig = "MtgRuInfoTable";

    public ParseService(IConfiguration fullConfig, MtgContext context)
    {
        _urlsConfig = fullConfig.GetSection("ExternalUrls");
        _context = context;
    }
    
    public async Task<Card> ParseOneCard(string cardName)
    {
        try
        {
            IDocument doc = await GetCardInfo(cardName);
            Card result = ParseDoc(doc);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
    private async Task<IDocument> GetCardInfo(string cardName)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        IAngleConfig config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(_urlsConfig[MtgRuInConfig] + cardName);
    }
    
    public Card ParseDoc(IDocument doc)
    {
        IHtmlCollection<IElement> cellsText = doc.QuerySelectorAll(CellSelectorMain);
        
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);

        return SplitDataToFields(cellsText: cellsText, cellsInfo: cellsInfo);
    }
    
    private Card SplitDataToFields( IHtmlCollection<IElement> cellsText,  IHtmlCollection<IElement> cellsInfo)
    {
        IHtmlImageElement? img = cellsInfo[0].QuerySelectorAll("img").FirstOrDefault() as IHtmlImageElement;
        (string cmc, string color) cmcColor = GetManaCostAndColor(cellsInfo[2]);
        (string power, string toughness) powerAndTough = GetPowerAndToughness(cellsInfo[3]);
        (List<string> keywords, string text) keywordsAndText = GetKeywordsAndText(cellsText[0]);
        List<Keyword> keywords = keywordsAndText.keywords.Select(x => _context.Keywords
                                                         .FirstOrDefault(y => x.Contains(y.Name) || y.RusName == x))
                                                         .Where(x => x != null)
                                                         .ToList()!;
        
        string rarityText = GetSubstringAfterChar(cellsInfo[4].TextContent, '-').Trim();
        Rarity rarity = _context.Rarities.First(x => x.RusName == rarityText || x.Name == rarityText);
        bool isRusCard = cellsText.Length > 1;
        Card result = new()
        {
            Name = cellsInfo[0].TextContent.Trim(),
            Img = img?.Source,
            Type = GetSubstringAfterChar(cellsInfo[1].TextContent.Replace("\n", String.Empty),':').Trim(),
            Text = keywordsAndText.text,
            TextRus = isRusCard ? cellsText[1].TextContent:String.Empty,
            IsRus = isRusCard,
            Power = powerAndTough.power,
            Toughness = powerAndTough.toughness,
            Cmc =  cmcColor.cmc,
            Color = cmcColor.color,
            Keywords = keywords
        };

        return result;
    }

    private (List<string> keywords, string text) GetKeywordsAndText(IElement element)
    {
        var closeTag = element.InnerHtml.IndexOf('<');
        if (closeTag < 0)
        {
            return (new List<String>(), element.TextContent);
        }
        
        string allKeywords = element.InnerHtml[..closeTag];
        List<string> keywordsResult = allKeywords.Split(',', StringSplitOptions.TrimEntries)
                                                 .Where(x => !string.IsNullOrWhiteSpace(x))
                                                 .ToList();
        
        string textResult = element.TextContent[allKeywords.Length..];
        
        return (keywordsResult, textResult);
    }

    private static string GetSubstringAfterChar(string text, params char[] separators)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (separators.Contains(text[i]))
                return text[(i+1)..];
        }

        return text;
    }

    private static (string power, string toughness) GetPowerAndToughness(IElement source)
    {
        string powerAndTough = GetSubstringAfterChar(source.TextContent, ':');
        int separator = powerAndTough.IndexOf('/');
        if (separator < 0)
        {
            return ("-", "-");
        }
        
        return (powerAndTough[..^separator].Trim(), powerAndTough[(separator + 1)..].Trim());
    }

    private static (string cmc, string color) GetManaCostAndColor(IElement source)
    {
        List<string> allData = source.QuerySelectorAll(".Mana")
                                     .Select(x => (x as IHtmlImageElement)?.AlternativeText)
                                     .Where(x => !String.IsNullOrEmpty(x))
                                     .ToList()!;

        List<string> allColorData = allData.Where(x => x.All(y => !IsDigitOrX(y))).ToList();
        
        String color = String.Join(", ", allColorData.Distinct());
        if (String.IsNullOrWhiteSpace(color))
        {
            color = "-";
        }
        
        Boolean isHaveAnyX = allData.Any(x => x[0] == 'X');

        int cmc = allColorData.Count;
        if (Int32.TryParse(allData.FirstOrDefault(x => x.All(y => y.IsDigit())), out int digit))
            cmc += digit;

        string cmcResult = isHaveAnyX ? "X" : String.Empty;
        if (!string.IsNullOrEmpty(cmcResult))
            cmcResult += ", ";
            
        cmcResult += cmc.ToString();

        return (cmcResult, color);
    }

    private static bool IsDigitOrX(char symbol)
    {
        return symbol.IsDigit() || symbol == 'X';
    }
}
