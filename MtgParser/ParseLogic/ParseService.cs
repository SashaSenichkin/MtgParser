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

    /// <summary>
    /// Получение цены для физической карты
    /// </summary>
    /// <param name="cardSet">ссылка на физическую карту. фактически, достаточно названия и аббревиатуры сета</param>
    /// <returns>цена карты</returns>
    /// <exception cref="Exception">полученные исключение просто перебрасываются выше, с выводом в консоль</exception>
    public async Task<Price> GetPriceAsync(CardSet cardSet)
    {
        try
        {
            string searchCardName = cardSet.Card.Name.Replace(' ', '+');
            IDocument doc = await GetHtmlAsync($"{_urlsConfig[GoldfishPriceConfig] + cardSet.Set.SearchText}/{searchCardName}" );
 
            Price? result = GetParsedPrice(doc);
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
            string? setName = cardName.Name ?? cardName.NameRus;
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
        if (fullTable.First().QuerySelector("img") is IHtmlImageElement img)
        {
            result.Img = img.Source.Replace("_middle", "");
        }

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
        
        string rarityText = GetSubStringAfterChar(cellsInfo[4].TextContent, '-').Trim();
        result.Rarity = _context.Rarities.First(x => x.RusName == rarityText || x.Name == rarityText);

        return result;
    }

    private CardSet GetOrCreateCardSet(Set set, Card card)
    {
        CardSet? result = _context.CardsSets.FirstOrDefault(x => x.Set.Equals(set) && x.Card == card);
        if (result != null)
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
        IEnumerable<string> splinted = manySetsInfo.InnerHtml.Split("ShowCardVersion").Skip(1).Select(x => GetSubStringAfterChar(x, ','));
        IEnumerable<string> cardVersions = splinted.Select(x => x[..x.IndexOf(',')].Trim());

        foreach (string cardVersion in cardVersions)
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
        IHtmlImageElement? imgData = element.QuerySelector("img") as IHtmlImageElement;
        
        Set? set = _context.Sets.FirstOrDefault(x => x.ShortName == imgData.AlternativeText);
        if (set != null)
        {
            return set;
        }
        
        if (imgData.AlternativeText == null || imgData.Source == null)
        {
            throw new Exception($"can't create set.. not enough data in " + element);
        }
        
        Set newSet = new()
        {
            ShortName = imgData.AlternativeText,
            SetImg = imgData.Source
        };

        (string main, string substr) = GetSeparateString(imgData.Title);

        newSet.FullName = main;
        newSet.RusName = substr;

        newSet.SearchText = newSet.FullName.Replace(":", string.Empty).Replace(' ', '+');

        _context.Sets.Add(newSet);
        _context.SaveChanges();
        return newSet;
    }
    
    private static Price? GetParsedPrice(IDocument doc)
    {
        IElement? priceBox = doc.QuerySelector(".price-box-price");
        string allDigits = GetSubStringAfterChar(priceBox.InnerHtml, ';');
        
        const NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
        CultureInfo provider = new ("en-GB");
        
        if (decimal.TryParse(allDigits,style, provider, out decimal price))
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
        card.TextRus = isRusCard ? cellsText[1].TextContent : string.Empty;
        card.IsRus = isRusCard;
        card.Keywords = keywords;
    }

    private static void SetCardData(Card card, IHtmlCollection<IElement> cellsInfo)
    {
        (string cmc, string color) = GetManaCostAndColor(cellsInfo[2]);
        (string power, string toughness) = GetPowerAndToughness(cellsInfo[3]);

        string cardTypePart = cellsInfo[1].TextContent.Replace("\n", string.Empty).Trim();
        (string typeMain, string typeSubstr) = GetSeparateString(GetSubStringAfterChar(cardTypePart,':'));
        
        (string nameMain, string nameSubstr) = GetSeparateString(cellsInfo[0].TextContent);

        
        card.Power = power;
        card.Toughness = toughness;
        card.Cmc = cmc;
        card.Color = color;
        card.Name = nameMain;
        card.NameRus = nameSubstr;
        card.Type = typeMain;
        card.TypeRus = typeSubstr;
    }
    
    private static (List<string> keywords, string text) GetKeywordsAndText(IElement element)
    {
        int closeTag = element.InnerHtml.IndexOf('<');
        if (closeTag < 0)
        {
            return (new List<string>(), element.TextContent);
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
        string powerAndTough = GetSubStringAfterChar(source.TextContent, ':');
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
                                     .Where(x => !string.IsNullOrEmpty(x))
                                     .ToList()!;

        List<string> allColorData = allData.Where(x => x.All(y => !IsDigitOrX(y))).ToList();
        
        string color = string.Join(", ", allColorData.Distinct());
        if (string.IsNullOrWhiteSpace(color))
        {
            color = "-";
        }
        
        bool isHaveAnyX = allData.Any(x => x[0] == 'X');

        int cmc = allColorData.Count;
        if (int.TryParse(allData.FirstOrDefault(x => x.All(y => y.IsDigit())), out int digit))
            cmc += digit;

        string cmcResult = isHaveAnyX ? "X" : string.Empty;
        if (!string.IsNullOrEmpty(cmcResult))
            cmcResult += ", ";
            
        cmcResult += cmc.ToString();

        return (cmcResult, color);
    }

    private static bool IsDigitOrX(char symbol)
    {
        return symbol.IsDigit() || symbol == 'X';
    }

    private static (string main, string substr) GetSeparateString(string? source, string separator = "//")
    {
        if (string.IsNullOrEmpty(source))
        {
            throw new ArgumentException("source can't be null");
        }
        
        int separatorIndex = source.IndexOf(separator, StringComparison.Ordinal);
        if (separatorIndex < 0)
        {
            return (source, string.Empty);
        }
        
        string left = source[..separatorIndex].Trim();
        string right = source[separatorIndex..].Trim('/', ' ');
        return left != right ? (left, right) : (left, string.Empty);
    }

    private static string GetSubStringAfterChar(string text, params char[] separators)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (separators.Contains(text[i]))
                return text[(i+1)..];
        }

        return text.Trim();
    }
}
