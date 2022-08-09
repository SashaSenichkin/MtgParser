using System.Globalization;
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
    private const string GoldfishPriceConfig = "PriceApi";

    public ParseService(IConfiguration fullConfig, MtgContext context)
    {
        _urlsConfig = fullConfig.GetSection("ExternalUrls");
        _context = context;
    }

    public async Task<Price> GetPriceAsync(CardSet cardSet)
    {
        try
        {
            String searchCardName = cardSet.Card.Name.Replace(' ', '+');
            IDocument doc = await GetHtmlAsync($"{_urlsConfig[GoldfishPriceConfig] + cardSet.Set.SearchText}/{searchCardName}" );
            Price result = GetParsedPrice(doc);
            if (result == null)
            {
                Console.WriteLine("Can't parse price data " + cardSet.Id );
                throw new Exception();
            }
            
            result.CardSet = cardSet;
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
    
    public async Task<Card> GetCardAsync(string cardName)
    {
        try
        {
            IDocument doc = await GetHtmlAsync(_urlsConfig[MtgRuInConfig] + cardName);
            Card result = GetParsedCard(doc);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<CardSet> GetCardSetAsync(CardName cardName)
    {
        try
        {
            String? setName = cardName.Name ?? cardName.NameRus;
            IDocument doc = await GetHtmlAsync(_urlsConfig[MtgRuInConfig] + setName);
            CardSet result = GetParsedCardSet(doc, cardName);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private async Task<IDocument> GetHtmlAsync(string path)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        IAngleConfig config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(path);
    }

    public Card GetParsedCard(IDocument doc)
    {
        Card result = new ();
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);
        SetCardData(result, cellsInfo);
        
        IHtmlCollection<IElement> cellsText = doc.QuerySelectorAll(CellSelectorMain);
        SetCardTextAndKeywords(result, cellsText);
        
        IHtmlCollection<IElement> fullTable = doc.QuerySelectorAll(FullTableInfo);
        IHtmlImageElement img = fullTable.First().QuerySelector("img") as IHtmlImageElement;
        result.Img = img.Source.Replace("_middle", "");

        return result;
    }
    
    private CardSet GetParsedCardSet(IDocument doc, CardName fullCardInfo)
    {
        Card card = GetParsedCard(doc);
        card.IsRus = fullCardInfo.NameRus == null;
        
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);
        
        Set set = GetParsedSet(fullCardInfo, cellsInfo);
        CardSet result = GetOrCreateCardSet(set, card);
        
        result.Quantity = fullCardInfo.Quantity;
        result.IsFoil = (byte)(fullCardInfo.IsFoil ? 1 : 0);
        
        string rarityText = GetSubstringAfterChar(cellsInfo[4].TextContent, '-').Trim();
        result.Rarity = _context.Rarities.First(x => x.RusName == rarityText || x.Name == rarityText);

        return result;
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
    
    private Set GetParsedSet(CardName cardName, IHtmlCollection<IElement> elements)
    {
        IElement manySetsInfo = elements[5];
        IEnumerable<String> splitted = manySetsInfo.InnerHtml.Split("ShowCardVersion").Skip(1).Select(x => GetSubstringAfterChar(x, ','));
        IEnumerable<String> cardVersions = splitted.Select(x => x[..x.IndexOf(',')].Trim());

        foreach (String cardVersion in cardVersions)
        {
            Task<IDocument> docT = GetHtmlAsync(_urlsConfig[MtgRuInfoTableConfig] + cardVersion);
            docT.Wait();
            IHtmlCollection<IElement> cellsInfo = docT.Result.QuerySelectorAll(CellSelectorInfo);
            Set set = GetOrCreateSet(cellsInfo[0]);
            if (set.ShortName == cardName.SetShort)
            {
                return set;
            }
        }
        
        return GetOrCreateSet(elements[0]);
    }

    private Set GetOrCreateSet(IElement element)
    {
        IHtmlImageElement imgData = (IHtmlImageElement) element.QuerySelector("img")!;
        Set? set = _context.Sets.FirstOrDefault(x => x.ShortName == imgData.AlternativeText);
        if (set != null)
        {
            return set;
        }
        
        Set newSet = new()
        {
            ShortName = imgData.AlternativeText,
            SetImg = imgData.Source
        };

        (String main, String substr) separated = GetSeparateString(imgData.Title);

        newSet.FullName = separated.main;
        newSet.RusName = separated.substr;

        newSet.SearchText = newSet.FullName.Replace(":", String.Empty).Replace(' ', '+');

        _context.Sets.Add(newSet);
        _context.SaveChanges();
        return newSet;
    }
    
    private static Price? GetParsedPrice(IDocument doc)
    {
        IElement? priceBox = doc.QuerySelector(".price-box-price");
        String allDigits = GetSubstringAfterChar(priceBox.InnerHtml, ';');
        
        const NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
        CultureInfo provider = new ("en-GB");
        
        if (Decimal.TryParse(allDigits,style, provider, out Decimal price))
        {
            return new Price() 
            {
                Value = price, 
                CreateDate = DateTime.Now
            };
        }

        return null;
    }


    private void SetCardTextAndKeywords(Card card, IHtmlCollection<IElement> cellsText)
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

    private static void SetCardData(Card card, IHtmlCollection<IElement> cellsInfo)
    {
        (string cmc, string color) = GetManaCostAndColor(cellsInfo[2]);
        (string power, string toughness) = GetPowerAndToughness(cellsInfo[3]);

        String cardTypePart = cellsInfo[1].TextContent.Replace("\n", String.Empty).Trim();
        (String main, String substr) typeSeparated = GetSeparateString(GetSubstringAfterChar(cardTypePart,':'));
        
        (String main, String substr) nameSeparated = GetSeparateString(cellsInfo[0].TextContent);

        
        card.Power = power;
        card.Toughness = toughness;
        card.Cmc = cmc;
        card.Color = color;
        card.Name = nameSeparated.main;
        card.NameRus = nameSeparated.substr;
        card.Type = typeSeparated.main;
        card.TypeRus = typeSeparated.substr;
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

    private static (string main, string substr) GetSeparateString(string source, string separator = "//")
    {
        int separatorIndex = source.IndexOf(separator);
        if (separatorIndex < 0)
        {
            return (source, String.Empty);
        }
        
        String left = source[..separatorIndex].Trim();
        String right = source[separatorIndex..].Trim('/', ' ');
        return left != right ? (left, right) : (left, String.Empty);
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
}
