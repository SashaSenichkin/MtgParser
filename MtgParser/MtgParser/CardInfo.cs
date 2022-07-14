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

    public CardInfo( IHtmlCollection<IElement> cellsText,  IHtmlCollection<IElement> cellsInfo)
    {
        Title = cellsInfo[0].TextContent;
        CardType = cellsInfo[1].TextContent;
        SummonCost = cellsInfo[2].TextContent;
        Str = cellsInfo[3].TextContent;
        Rarity = cellsInfo[4].TextContent;
        
        EngDescr = cellsText[0].TextContent;
        RusDescr = cellsText[1].TextContent;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"title = {Title}");
        sb.Append($"cardType = {CardType}");
        sb.Append($"summonCost = {SummonCost}");
        sb.Append($"str = {Str}");
        sb.Append($"rarity = {Rarity}");
        sb.Append($"engDescr = {EngDescr}");
        sb.Append($"rusDescr = {RusDescr}");
        return sb.ToString();
    }
}