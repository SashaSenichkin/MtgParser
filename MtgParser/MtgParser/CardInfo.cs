using System.Text;
using AngleSharp.Dom;

namespace MtgParser;

public class CardInfo
{
    public string Title;
    public string CardType;

    public string SummonCost;
    public string Str;
    public string Rarity;
    public string EngDescr;
    public string RusDescr;
    public string ImgPath;
    public string SetShortName;
    public string SetFullName;

    
    public CardInfo( IHtmlCollection<IElement> cellsText,  IHtmlCollection<IElement> cellsInfo)
    {
        Title = cellsInfo[0].TextContent.Trim();
        
        var img = cellsInfo[0].QuerySelectorAll("img").FirstOrDefault() as AngleSharp.Html.Dom.IHtmlImageElement;
        ImgPath = img?.Source;
        SetShortName = img?.AlternativeText;
        SetFullName = String.Concat(img.Title.TakeWhile(x => x != '/')).Trim();

        CardType = cellsInfo[1].TextContent.Replace("\n", String.Empty);
        SummonCost = String.Join(" ", cellsInfo[2].QuerySelectorAll(".Mana").Select(x=> (x as AngleSharp.Html.Dom.IHtmlImageElement)?.AlternativeText));
        Str = cellsInfo[3].TextContent;
        Rarity = cellsInfo[4].TextContent;
        
        EngDescr = cellsText[0].TextContent;
        RusDescr = cellsText[1].TextContent;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"title = {Title}");
        sb.AppendLine($"cardType = {CardType}");
        sb.AppendLine($"summonCost = {SummonCost}");
        sb.AppendLine($"str = {Str}");
        sb.AppendLine($"rarity = {Rarity}");
        sb.AppendLine($"engDescr = {EngDescr}");
        sb.AppendLine($"rusDescr = {RusDescr}");
        return sb.ToString();
    }
}