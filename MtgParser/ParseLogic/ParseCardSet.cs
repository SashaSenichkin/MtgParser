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

public class ParseCardSet : BaseParser
{
    private readonly IConfigurationSection _urlsConfig;
    private readonly MtgContext _context;

    private const string CellSelectorMain = ".SearchCardInfoText";
    private const string CellSelectorInfo = ".SearchCardInfoDIV";
    private const string FullTableInfo = ".NoteDiv";
    
    private const string MtgRuInConfig = "BaseMtgRu";
    private const string MtgRuInfoTableConfig = "MtgRuInfoTable";

    public ParseCardSet(IConfiguration fullConfig, MtgContext context)
    {
        _urlsConfig = fullConfig.GetSection("ExternalUrls");
        _context = context;
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
            Card? storedCard = _context.Cards.FirstOrDefault(x => x.Name.Equals(cardName.Name, StringComparison.InvariantCultureIgnoreCase)
                                                                   || x.NameRus.Equals(cardName.NameRus, StringComparison.InvariantCultureIgnoreCase));
            
            Set? storedSet = _context.Sets.FirstOrDefault(x => x.ShortName.Equals(cardName.SetShort, StringComparison.InvariantCultureIgnoreCase));

            if (storedCard != null && storedSet != null)
            {
                return GetOrCreateCardSet(storedSet, storedCard);
            }
            
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
    
    public Card GetParsedCard(IDocument doc)
    {
        Card result = new ();
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);
        SetCardData(result, cellsInfo);
        
        Card? storedCard = _context.Cards.FirstOrDefault(x => x.Name.Equals(result.Name, StringComparison.InvariantCultureIgnoreCase)
                                                               || x.NameRus.Equals(result.NameRus, StringComparison.InvariantCultureIgnoreCase));
        if (storedCard != null)
        {
            return storedCard;
        }

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
        result.Rarity = _context.Rarities.First(x => x.RusName.Equals(rarityText, StringComparison.InvariantCultureIgnoreCase) 
                                                       || x.Name.Equals(rarityText, StringComparison.InvariantCultureIgnoreCase));

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
    
    #region ParseFields
    
    private void SetCardTextAndKeywords(Card card, IHtmlCollection<IElement> cellsText)
    {
        (List<string> keywordsText, string text) = GetKeywordsAndText(cellsText[0]);
        List<Keyword> keywords = keywordsText.Select(x => _context.Keywords
                .FirstOrDefault(y => x.Contains(y.Name, StringComparison.InvariantCultureIgnoreCase) || y.RusName == x))
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
    #endregion
}
