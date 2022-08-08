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
    private const string FullTableInfo = ".NoteDiv";
    
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
            Card result = ParseCard(doc);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<CardSet> ParseOneCardSet(string cardName, string setShortName)
    {
        return await ParseOneCardSet(new CardName() { Name = cardName, SetShort = setShortName });
    }
    
    public async Task<CardSet> ParseOneCardSet(CardName cardName)
    {
        try
        {
            IDocument doc = await GetCardInfo(cardName.Name ?? cardName.NameRus);
            CardSet result = ParseCardSet(doc, cardName);
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
    
    private async Task<IDocument> GetSetInfo(string cardVersion)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        IAngleConfig config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(_urlsConfig[MtgRuInfoTableConfig] + cardVersion);
    }
    
    public Card ParseCard(IDocument doc)
    {
        Card result = new Card();
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);
        FillCardData(result, cellsInfo);
        
        IHtmlCollection<IElement> cellsText = doc.QuerySelectorAll(CellSelectorMain);
        FillCardText(result, cellsText);
        
        IHtmlCollection<IElement> fullTable = doc.QuerySelectorAll(FullTableInfo);
        IHtmlImageElement img = fullTable.First().QuerySelector("img") as IHtmlImageElement;
        result.Img = img.Source.Replace("_middle", "");

        return result;
    }
    
    private CardSet ParseCardSet(IDocument doc, CardName fullCardInfo)
    {
        Card card = ParseCard(doc);
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);
        
        Set set = GetCardSet(fullCardInfo, cellsInfo[5]);
        CardSet result = GetOrCreateCardSet(set, card);
        
        result.Quantity = fullCardInfo.Quantity;
        result.IsFoil = (byte)(fullCardInfo.IsFoil ? 1 : 0);
        
        string rarityText = GetSubstringAfterChar(cellsInfo[4].TextContent, '-').Trim();
        result.Rarity = _context.Rarities.First(x => x.RusName == rarityText || x.Name == rarityText);

        return result;
    }

    private Set GetCardSet(CardName cardName, IElement element)
    {
        IEnumerable<String> splitted = element.InnerHtml.Split("ShowCardVersion").Skip(1).Select(x => GetSubstringAfterChar(x, ','));
        IEnumerable<String> cardVersions = splitted.Select(x => x[..x.IndexOf(',')].Trim());

        foreach (String cardVersion in cardVersions)
        {
            Task<IDocument> docT = GetSetInfo(cardVersion);
            docT.Wait();
            IHtmlCollection<IElement> cellsInfo = docT.Result.QuerySelectorAll(CellSelectorInfo);
            Set set = GetOrCreateSet(cellsInfo[0]);
            if (set.ShortName == cardName.SetShort)
            {
                return set;
            }
        }

        return null;
    }

    private CardSet GetOrCreateCardSet(Set set, Card card)
    {
        CardSet result = _context.CardsSets.FirstOrDefault(x => x.Set.Equals(set) && x.Card == card);
        if (result!= null)
        {
            return result;
        }

        CardSet newCardSet = new()
        {
            Card = card,
            Set = set
        };
        return newCardSet;
    }

    private Set GetOrCreateSet(IElement element)
    {
        IHtmlImageElement imgData = (IHtmlImageElement) element.QuerySelector("img")!;
        Set? set = _context.Sets.FirstOrDefault(x => x.ShortName == imgData.AlternativeText);
        if (set != null)
        {
            return set;
        }
        
        int separatorIndex = imgData.Title.IndexOf("//");
        Set newSet = new Set()
        {
            FullName = imgData.Title[..separatorIndex].Trim(),
            ShortName = imgData.AlternativeText,
            SetImg = imgData.Source
        };
        
        newSet.SearchText = newSet.FullName.Replace(' ', '+');

        String titlePart = imgData.Title[separatorIndex..].Trim('/', ' ');
        if (newSet.FullName != titlePart)
        {
            newSet.RusName = titlePart;
        }

        _context.Sets.Add(newSet);
        _context.SaveChanges();
        return newSet;
    }

    private void FillCardText(Card card, IHtmlCollection<IElement> cellsText)
    {
        (List<string> keywordsText, string text) = GetKeywordsAndText(cellsText[0]);
        List<Keyword> keywords = keywordsText.Select(x => _context.Keywords
                .FirstOrDefault(y => x.Contains(y.Name) || y.RusName == x))
            .Where(x => x != null)
            .ToList()!;
        
        bool isRusCard = cellsText.Length > 1;
        card.Text = text;
        card.TextRus = isRusCard ? cellsText[1].TextContent : String.Empty;
        card.IsRus = isRusCard;
        card.Keywords = keywords;
    }

    private static void FillCardData(Card card, IHtmlCollection<IElement> cellsInfo)
    {
        (string cmc, string color) = GetManaCostAndColor(cellsInfo[2]);
        (string power, string toughness) = GetPowerAndToughness(cellsInfo[3]);

        card.Power = power;
        card.Toughness = toughness;
        card.Cmc = cmc;
        card.Color = color;
        card.Name = cellsInfo[0].TextContent.Trim();
        card.Type = GetSubstringAfterChar(cellsInfo[1].TextContent.Replace("\n", String.Empty), ':');
    }
    
    private static (List<string> keywords, string text) GetKeywordsAndText(IElement element)
    {
        int closeTag = element.InnerHtml.IndexOf('<');
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

        return text.Trim();
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
