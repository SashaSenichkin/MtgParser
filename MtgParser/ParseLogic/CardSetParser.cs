using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using MtgParser.Context;
using MtgParser.Model;

using IAngleConfig = AngleSharp.IConfiguration;


namespace MtgParser.ParseLogic;

public class CardSetParser : BaseParser
{
    private readonly MtgContext _context;

    private static readonly string[] CellSelectorMain = {".SearchCardInfoText", ".SearchCardTextDiv"};
    private const string CellSelectorInfo = ".SearchCardInfoDIV";
    private const string FullTableInfo = ".NoteDiv";
    
    public CardSetParser(MtgContext context)
    {
        _context = context;
    }

    public Card? GetCard(IDocument doc)
    {
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);
        if (!cellsInfo.Any())
        {
            return null;
        }
        
        Card result = new();
        SetCardData(result, cellsInfo);

        IHtmlCollection<IElement>? cellsText = null;
        foreach (string selector in CellSelectorMain)
        {
            cellsText = doc.QuerySelectorAll(selector);
            if (cellsText.Any())
            {
                break;
            }
        }

        if (!cellsText.Any())
        {
            throw new Exception($"can't find text node in {doc.Source}");
        }

        SetCardTextAndKeywords(result, cellsText);
        
        IHtmlCollection<IElement> fullTable = doc.QuerySelectorAll(FullTableInfo);
        if (fullTable.First().QuerySelector("img") is IHtmlImageElement img)
        {
            result.Img = img.Source.Replace("_middle", "");
        }

        return result;
    }
    
    public CardSet GetCardSet(IDocument doc)
    {
        CardSet result = new();
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);
        
        string rarityText = GetSubStringAfterChar(cellsInfo[4].TextContent, '-').Trim();
        result.Rarity = _context.Rarities.First(x => x.RusName.Equals(rarityText) || x.Name.Equals(rarityText));

        return result;
    }
    
    /// <summary>
    /// take all available sets info from html
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static (IEnumerable<string> variousSets, IElement defaultOption) GetSetCandidates(IDocument doc)
    {
        IHtmlCollection<IElement> elements = doc.QuerySelectorAll(CellSelectorInfo);
        IElement manySetsInfo = elements[5];
        IEnumerable<string> splinted = manySetsInfo.InnerHtml.Split("ShowCardVersion").Skip(1).Select(x => GetSubStringAfterChar(x, ','));
        IEnumerable<string> cardVersions = splinted.Select(x => x[..x.IndexOf(',')].Trim());

        return (cardVersions, elements[0]);
    }


    public Set GetSet(IDocument doc)
    {
        IHtmlCollection<IElement> cellsInfo = doc.QuerySelectorAll(CellSelectorInfo);
        return GetSet(cellsInfo.First());
    }

    public Set GetSet(IElement element)
    {
        IHtmlImageElement? imgData = element.QuerySelector("img") as IHtmlImageElement;
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
                .FirstOrDefault(y => x.Contains(y.Name) || y.RusName == x))
            .Where(x => x != null)
            .ToList()!;
        
        card.Text = text;
        if (cellsText.Length > 1)
        {
            card.TextRus = cellsText[1].TextContent;
        }
        
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
